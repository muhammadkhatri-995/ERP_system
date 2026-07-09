using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ERP_sys.Data;
using ERP_sys.Models;

namespace ERP_sys.Repositories
{
    public class SalesLedgerRepository : ISalesLedgerRepository
    {
        private readonly string _connectionString;

        public SalesLedgerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateInvoiceAsync(SalesLedger ledger)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                int newLedgerId;

                using (var cmd = new SqlCommand(@"
                    INSERT INTO SalesLedger
                        (InvoiceNo, CustomerId, CustomerName, InvoiceDate, DueDate, Subtotal, Discount, Tax,
                         GrandTotal, PaidAmount, Balance, PaymentStatus, Notes, CreatedBy, CreatedDate, IsDeleted)
                    OUTPUT INSERTED.Id
                    VALUES
                        (@InvoiceNo, @CustomerId, @CustomerName, @InvoiceDate, @DueDate, @Subtotal, @Discount, @Tax,
                         @GrandTotal, @PaidAmount, @Balance, @PaymentStatus, @Notes, @CreatedBy, @CreatedDate, 0)",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", ledger.InvoiceNo);
                    cmd.Parameters.AddWithValue("@CustomerId", ledger.CustomerId);
                    cmd.Parameters.AddWithValue("@CustomerName", ledger.CustomerName);
                    cmd.Parameters.AddWithValue("@InvoiceDate", ledger.InvoiceDate);
                    cmd.Parameters.AddWithValue("@DueDate", (object)ledger.DueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Subtotal", ledger.Subtotal);
                    cmd.Parameters.AddWithValue("@Discount", ledger.Discount);
                    cmd.Parameters.AddWithValue("@Tax", ledger.Tax);
                    cmd.Parameters.AddWithValue("@GrandTotal", ledger.GrandTotal);
                    cmd.Parameters.AddWithValue("@PaidAmount", ledger.PaidAmount);
                    cmd.Parameters.AddWithValue("@Balance", ledger.GrandTotal - ledger.PaidAmount);
                    cmd.Parameters.AddWithValue("@PaymentStatus", ledger.PaymentStatus ?? "Unpaid");
                    cmd.Parameters.AddWithValue("@Notes", (object)ledger.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", ledger.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    newLedgerId = (int)await cmd.ExecuteScalarAsync();
                }

                foreach (var item in ledger.SalesLedgerItem)
                {
                    using var itemCmd = new SqlCommand(@"
                        INSERT INTO SalesLedgerItems
                            (SalesLedgerId, ProductId, Quantity, UnitPrice, Discount, Tax, LineTotal)
                        VALUES
                            (@SalesLedgerId, @ProductId, @Quantity, @UnitPrice, @Discount, @Tax, @LineTotal)",
                        connection, transaction);

                    itemCmd.Parameters.AddWithValue("@SalesLedgerId", newLedgerId);
                    itemCmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                    itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    itemCmd.Parameters.AddWithValue("@Discount", item.Discount);
                    itemCmd.Parameters.AddWithValue("@Tax", item.Tax);
                    itemCmd.Parameters.AddWithValue("@LineTotal", item.LineTotal);

                    await itemCmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();
                return newLedgerId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<SalesLedger> GetInvoiceByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            SalesLedger ledger = null;

            using (var cmd = new SqlCommand(@"
                SELECT sl.*, c.Id AS Customer_Id
                FROM SalesLedger sl
                INNER JOIN Customers c ON sl.CustomerId = c.Id
                WHERE sl.Id = @Id AND sl.IsDeleted = 0",
                connection))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    ledger = new SalesLedger
                    {
                        Id = (int)reader["Id"],
                        InvoiceNo = reader["InvoiceNo"].ToString(),
                        CustomerId = (int)reader["CustomerId"],
                        CustomerName = reader["CustomerName"].ToString(),
                        InvoiceDate = (DateTime)reader["InvoiceDate"],
                        DueDate = reader["DueDate"] as DateTime?,
                        Subtotal = (decimal)reader["Subtotal"],
                        Discount = (decimal)reader["Discount"],
                        Tax = (decimal)reader["Tax"],
                        GrandTotal = (decimal)reader["GrandTotal"],
                        PaidAmount = (decimal)reader["PaidAmount"],
                        Balance = (decimal)reader["Balance"],
                        PaymentStatus = reader["PaymentStatus"].ToString(),
                        Notes = reader["Notes"] as string,
                        Customers = new Customers { Id = (int)reader["Customer_Id"] }
                    };
                }
            }

            if (ledger == null) return null;

            using (var cmd = new SqlCommand(@"
                SELECT sli.*, p.Id AS Product_Id, p.Name AS Product_Name
                FROM SalesLedgerItems sli
                INNER JOIN Products p ON sli.ProductId = p.Id
                WHERE sli.SalesLedgerId = @LedgerId",
                connection))
            {
                cmd.Parameters.AddWithValue("@LedgerId", id);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ledger.SalesLedgerItem.Add(new SalesLedgerItem
                    {
                        Id = (int)reader["Id"],
                        SalesLedgerId = (int)reader["SalesLedgerId"],
                        ProductId = (int)reader["ProductId"],
                        Quantity = (decimal)reader["Quantity"],
                        UnitPrice = (decimal)reader["UnitPrice"],
                        Discount = (decimal)reader["Discount"],
                        Tax = (decimal)reader["Tax"],
                        LineTotal = (decimal)reader["LineTotal"],
                        Products = new Products
                        {
                            Id = (int)reader["Product_Id"],
                            Name = reader["Product_Name"].ToString()
                        }
                    });
                }
            }

            return ledger;
        }

        public async Task<List<SalesLedger>> GetAllInvoicesAsync()
        {
            var invoices = new List<SalesLedger>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT *
                FROM SalesLedger
                WHERE IsDeleted = 0
                ORDER BY InvoiceDate DESC",
                connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                invoices.Add(new SalesLedger
                {
                    Id = (int)reader["Id"],
                    InvoiceNo = reader["InvoiceNo"].ToString(),
                    CustomerId = (int)reader["CustomerId"],
                    CustomerName = reader["CustomerName"].ToString(),
                    //  InvoiceDate = (DateTime)reader["InvoiceDate"],
                    invoice.DueDate = reader.IsDBNull(reader.GetOrdinal("DueDate"))
    ? null
    : reader.GetDateTime(reader.GetOrdinal("DueDate"));
                    DueDate = (DateTime?)reader["DueDate"],
                    GrandTotal = (decimal)reader["GrandTotal"],
                    PaidAmount = (decimal)reader["PaidAmount"],
                    Balance = (decimal)reader["Balance"],
                    PaymentStatus = reader["PaymentStatus"].ToString()
                });
            }

            return invoices;
        }

        public async Task<int> UpdateInvoiceAsync(SalesLedger ledger)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (var cmd = new SqlCommand(@"
                    UPDATE SalesLedger
                    SET
                        CustomerId = @CustomerId,
                        CustomerName = @CustomerName,
                        InvoiceDate = @InvoiceDate,
                        DueDate = @DueDate,
                        Subtotal = @Subtotal,
                        Discount = @Discount,
                        Tax = @Tax,
                        GrandTotal = @GrandTotal,
                        PaidAmount = @PaidAmount,
                        Balance = @Balance,
                        PaymentStatus = @PaymentStatus,
                        Notes = @Notes,
                        UpdatedBy = @UpdatedBy,
                        UpdatedDate = @UpdatedDate
                    WHERE Id = @Id",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", ledger.Id);
                    cmd.Parameters.AddWithValue("@CustomerId", ledger.CustomerId);
                    cmd.Parameters.AddWithValue("@CustomerName", ledger.CustomerName);
                    cmd.Parameters.AddWithValue("@InvoiceDate", ledger.InvoiceDate);
                    cmd.Parameters.AddWithValue("@DueDate", (object)ledger.DueDate ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Subtotal", ledger.Subtotal);
                    cmd.Parameters.AddWithValue("@Discount", ledger.Discount);
                    cmd.Parameters.AddWithValue("@Tax", ledger.Tax);
                    cmd.Parameters.AddWithValue("@GrandTotal", ledger.GrandTotal);
                    cmd.Parameters.AddWithValue("@PaidAmount", ledger.PaidAmount);
                    cmd.Parameters.AddWithValue("@Balance", ledger.GrandTotal - ledger.PaidAmount);
                    cmd.Parameters.AddWithValue("@PaymentStatus", ledger.PaymentStatus);
                    cmd.Parameters.AddWithValue("@Notes", (object)ledger.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedBy", ledger.UpdatedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = new SqlCommand(
                    "DELETE FROM SalesLedgerItems WHERE SalesLedgerId = @Id",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", ledger.Id);
                    await cmd.ExecuteNonQueryAsync();
                }
                
                foreach (var item in ledger.SalesLedgerItem)
                {
                    using var cmd = new SqlCommand(@"
                        INSERT INTO SalesLedgerItems
                        (SalesLedgerId, ProductId, Quantity, UnitPrice, Discount, Tax, LineTotal)
                        VALUES
                        (@SalesLedgerId, @ProductId, @Quantity, @UnitPrice, @Discount, @Tax, @LineTotal)",
                        connection, transaction);

                    cmd.Parameters.AddWithValue("@SalesLedgerId", ledger.Id);
                    cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@UnitPrice", item.UnitPrice);
                    cmd.Parameters.AddWithValue("@Discount", item.Discount);
                    cmd.Parameters.AddWithValue("@Tax", item.Tax);
                    cmd.Parameters.AddWithValue("@LineTotal", item.LineTotal);

                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();

                return ledger.Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<int> DeleteInvoiceAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                DELETE FROM SalesLedger WHERE Id = @Id",
                connection);

            cmd.Parameters.AddWithValue("@Id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}