using System;
using System.Collections.Generic;

namespace ERP_sys.Models
{
    public class Products
    {
        public int Id { get; set; }

     
        public string Name { get; set; } = string.Empty;

    
        public string? SKU { get; set; }

        public string? Category { get; set; }

    
        public decimal Price { get; set; }


        public decimal StockQuantity { get; set; }

        
        public string? Unit { get; set; }
    }
}