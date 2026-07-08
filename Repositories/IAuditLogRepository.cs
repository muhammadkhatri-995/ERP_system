using ERP_sys.Models;
using ERP_sys.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP_sys.Repositories
{
    public interface IAuditLogRepository
    {
        Task InsertAsync(AuditLogs log);
        Task<List<AuditLogs>> GetLogsAsync(AuditLogFilterDto filter);
    }

}
