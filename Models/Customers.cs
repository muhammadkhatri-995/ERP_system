using System;
using System.Collections.Generic;
namespace ERP_sys.Models
{
    public class Customers
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
       public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        // Navigation property for SalesLedger
        public List<SalesLedger> SalesLedgers { get; set; } = new List<SalesLedger>();

    }
}