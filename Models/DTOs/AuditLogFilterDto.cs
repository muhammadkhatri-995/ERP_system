using System;

namespace ERP_sys.Models.DTOs
{
    public class AuditLogFilterDto
    {
        public string? IpAddress { get; set; }
        public string? UserName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}