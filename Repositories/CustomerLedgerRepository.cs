using ERP_sys.Models;
using ERP_sys.Models.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
namespace ERP_sys.Repositories
{
	public class CustomerLedgerRepository : ICustomerLedgerRepository
	{
		private readonly string _connectionString;

		public CustomerLedgerRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DefaultConnection")!;
		}

		public async Task<CustomerLedgerResponseDto?> GetCustomerLedgerAsync(CustomerLedgerFilterDto filter)
		{
			using var conn = new SqlConnection(_connectionString);
			await conn.OpenAsync();

			// ---- Step 1: Get opening balance (before FromDate) ----
			decimal openingBalance = 0;
			if (filter.FromDate.HasValue)
			{
				using var obCmd = new SqlCommand("sp_GetOpeningBalance", conn) { CommandType = CommandType.StoredProcedure };
				obCmd.Parameters.AddWithValue("@CustomerId", filter.CustomerId);
				obCmd.Parameters.AddWithValue("@FromDate", filter.FromDate.Value);
				var result = await obCmd.ExecuteScalarAsync();
				openingBalance = result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
			}

			// ---- Step 2: Get invoices, payments, customer info ----
			var invoices = new List<(int Id, string InvoiceNo, DateTime TransactionDate, decimal GrandTotal, decimal PaidAmount, decimal Balance, string PaymentStatus, string? Notes)>();
			var payments = new List<(int Id, int? SalesLedgerId, DateTime TransactionDate, decimal Amount, string? PaymentMethod, string? ReferenceNo, string? Notes)>();
			CustomerInfoDto? customerInfo = null;

			using (var cmd = new SqlCommand("sp_GetCustomerLedgerData", conn) { CommandType = CommandType.StoredProcedure })
			{
				cmd.Parameters.AddWithValue("@CustomerId", filter.CustomerId);
				cmd.Parameters.AddWithValue("@FromDate", (object?)filter.FromDate ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@ToDate", (object?)filter.ToDate ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@InvoiceNo", (object?)filter.InvoiceNo ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@PaymentStatus", (object?)filter.PaymentStatus ?? DBNull.Value);

				using var reader = await cmd.ExecuteReaderAsync();

				// Result Set 1: Invoices
				while (await reader.ReadAsync())
				{
					invoices.Add((
						Id: (int)reader["Id"],
						InvoiceNo: reader["InvoiceNo"].ToString()!,
						TransactionDate: (DateTime)reader["TransactionDate"],
						GrandTotal: (decimal)reader["GrandTotal"],
						PaidAmount: (decimal)reader["PaidAmount"],
						Balance: (decimal)reader["Balance"],
						PaymentStatus: reader["PaymentStatus"].ToString()!,
						Notes: reader["Notes"] as string
					));
				}

				// Result Set 2: Payments
				await reader.NextResultAsync();
				while (await reader.ReadAsync())
				{
					payments.Add((
						Id: (int)reader["Id"],
						SalesLedgerId: reader["SalesLedgerId"] as int?,
						TransactionDate: (DateTime)reader["TransactionDate"],
						Amount: (decimal)reader["Amount"],
						PaymentMethod: reader["PaymentMethod"] as string,
						ReferenceNo: reader["ReferenceNo"] as string,
						Notes: reader["Notes"] as string
					));
				}

				// Result Set 3: Customer Info
				await reader.NextResultAsync();
				if (await reader.ReadAsync())
				{
					customerInfo = new CustomerInfoDto
					{
						Id = (int)reader["Id"],
						Name = reader["Name"].ToString()!,
						Phone = reader["Phone"] as string,
						Email = reader["Email"] as string,
						Address = reader["Address"] as string,
						CreditLimit = (decimal)reader["CreditLimit"]
					};
				}
			}

			if (customerInfo == null)
				return null;

			// ---- Step 3: Build combined ledger entries ----
			var entries = new List<CustomerLedgerEntryDto>();

			foreach (var inv in invoices)
			{
				entries.Add(new CustomerLedgerEntryDto
				{
					TransactionDate = inv.TransactionDate,
					VoucherType = "Invoice",
					InvoiceNo = inv.InvoiceNo,
					InvoiceId = inv.Id,
					Description = $"Invoice {inv.InvoiceNo}",
					Debit = inv.GrandTotal,
					Credit = 0,
					PaymentStatus = inv.PaymentStatus
				});

				if (inv.PaidAmount > 0)
				{
					entries.Add(new CustomerLedgerEntryDto
					{
						TransactionDate = inv.TransactionDate,
						VoucherType = "Payment",
						InvoiceNo = inv.InvoiceNo,
						InvoiceId = inv.Id,
						Description = $"Payment received against {inv.InvoiceNo}",
						Debit = 0,
						Credit = inv.PaidAmount,
						PaymentStatus = inv.PaymentStatus
					});
				}
			}

			foreach (var pay in payments)
			{
				entries.Add(new CustomerLedgerEntryDto
				{
					TransactionDate = pay.TransactionDate,
					VoucherType = "Payment",
					InvoiceNo = null,
					InvoiceId = pay.SalesLedgerId,
					Description = string.IsNullOrWhiteSpace(pay.ReferenceNo)
						? "Additional payment received"
						: $"Payment received (Ref: {pay.ReferenceNo})",
					Debit = 0,
					Credit = pay.Amount,
					PaymentStatus = null
				});
			}

			
			entries.Sort((a, b) =>
			{
				int dateCompare = a.TransactionDate.CompareTo(b.TransactionDate);
				if (dateCompare != 0) return dateCompare;
				return a.VoucherType == "Invoice" && b.VoucherType == "Payment" ? -1 :
					   a.VoucherType == "Payment" && b.VoucherType == "Invoice" ? 1 : 0;
			});

			// ---- Step 4: Compute running balance ----
			decimal runningBalance = openingBalance;
			foreach (var entry in entries)
			{
				runningBalance = runningBalance + entry.Debit - entry.Credit;
				entry.RunningBalance = runningBalance;
			}

			// ---- Step 5: Build summary ----
			decimal totalSales = 0, totalPaid = 0;
			int totalInvoiceCount = invoices.Count;

			foreach (var inv in invoices)
			{
				totalSales += inv.GrandTotal;
				totalPaid += inv.PaidAmount;
			}
			foreach (var pay in payments)
			{
				totalPaid += pay.Amount;
			}

			var summary = new CustomerLedgerSummaryDto
			{
				TotalInvoices = totalInvoiceCount,
				TotalSales = totalSales,
				TotalPaid = totalPaid,
				OutstandingAmount = runningBalance
			};

			customerInfo.OutstandingBalance = runningBalance;

			return new CustomerLedgerResponseDto
			{
				CustomerInfo = customerInfo,
				Summary = summary,
				LedgerEntries = entries
			};
		}
	}
}
