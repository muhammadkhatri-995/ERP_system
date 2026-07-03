using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace ERP_sys.Models
{
    public class SalesLedgerItem
    {
        public int Id { get; set; }
        public int SalesLedgerId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal LineTotal { get; set; }


        // Navigation property for SalesLedger
        [ValidateNever]
        public SalesLedger SalesLedger { get; set; }
        [ValidateNever]
        public Products? Products { get; set; }


    }
}