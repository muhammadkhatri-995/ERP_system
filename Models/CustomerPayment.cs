using System;
namespace ERP_sys.Models
{
    public class CustomerPayment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int? SalesLedgerId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Notes { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}