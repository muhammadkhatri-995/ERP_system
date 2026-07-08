using ERP_sys.Models.DTOs;
using ERP_sys.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ERP_sys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? ipAddress,
            [FromQuery] string? userName,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var filter = new AuditLogFilterDto
            {
                IpAddress = ipAddress,
                UserName = userName,
                FromDate = fromDate,
                ToDate = toDate
            };

            var logs = await _auditLogRepository.GetLogsAsync(filter);
            return Ok(logs);
        }
    }
}