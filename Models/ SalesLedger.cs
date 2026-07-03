using System;
using System.Collections.Generic;
namespace ERP_sys.Models
{
    public class SalesLedger
    {
        public int Id { get; set; }
        public String InvoiceNo { get; set; }
        public int CustomerId { get; set; } = 0;
        public String CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
        public String PaymentStatus { get; set; }
        public String Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;


        // Navigation property for Customer
        public Customers? Customers { get; set; }
        public List<SalesLedgerItem> SalesLedgerItem { get; set; } = new List<SalesLedgerItem>();





    }
}

//using System;
//using System.Collections.Generic;

//namespace ERP_sys.Models
//{
//    public class SalesLedger
//    {
//        public int Id { get; set; }
//        public String InvoiceNo { get; set; }
//        public int CustomerId { get; set; } = 0;
//        public String CustomerName { get; set; }
//        public DateTime InvoiceDate { get; set; } = DateTime.Now;
//        public DateTime? DueDate { get; set; } = DateTime.Now;
//        public decimal Subtotal { get; set; }
//        public decimal Discount { get; set; }
//        public decimal Tax { get; set; }
//        public decimal GrandTotal { get; set; }
//        public decimal PaidAmount { get; set; }
//        public decimal Balance { get; set; }
//        public String PaymentStatus { get; set; }
//        public String Notes { get; set; }
//        public int CreatedBy { get; set; }
//        public DateTime CreatedDate { get; set; } = DateTime.Now;
//        public int? UpdatedBy { get; set; }
//        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
//        public bool IsDeleted { get; set; } = false;

//        // Navigation property for Customer
//        public Customers? Customers { get; set; }

//        // Renamed from "SalesLedgerItem" to "Items" — matches SalesLedgerRepository.cs,
//        // and avoids a property sharing the exact name of its own item type.
//        public List<SalesLedgerItem> Items { get; set; } = new List<SalesLedgerItem>();
//    }
//}