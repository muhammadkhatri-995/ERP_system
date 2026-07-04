using System;
using System.Collections.Generic;
namespace ERP_sys.Models.DTOs
{
    
        public class CustomerLedgerFilterDto
        {
            public int CustomerId { get; set; }
            public DateTime? FromDate { get; set; }
            public DateTime? ToDate { get; set; }
            public string? PaymentStatus { get; set; }
            public string? InvoiceNo { get; set; }
        }

        public class CustomerInfoDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Phone { get; set; }
            public string? Email { get; set; }
            public string? Address { get; set; }
            public decimal CreditLimit { get; set; }
            public decimal OutstandingBalance { get; set; }
        }

        public class CustomerLedgerEntryDto
        {
            public DateTime TransactionDate { get; set; }
            public string VoucherType { get; set; } = string.Empty; // "Invoice" or "Payment"
            public string? InvoiceNo { get; set; }
            public int? InvoiceId { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal Debit { get; set; }
            public decimal Credit { get; set; }
            public decimal RunningBalance { get; set; }
            public string? PaymentStatus { get; set; }
        }

        public class CustomerLedgerSummaryDto
        {
            public int TotalInvoices { get; set; }
            public decimal TotalSales { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal OutstandingAmount { get; set; }
        }

        public class CustomerLedgerResponseDto
        {
            public CustomerInfoDto CustomerInfo { get; set; } = new CustomerInfoDto();
            public CustomerLedgerSummaryDto Summary { get; set; } = new CustomerLedgerSummaryDto();
            public List<CustomerLedgerEntryDto> LedgerEntries { get; set; } = new List<CustomerLedgerEntryDto>();
        }
    
}
