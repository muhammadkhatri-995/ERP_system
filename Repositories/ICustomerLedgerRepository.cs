using ERP_sys.Models.DTOs;
using System.Threading.Tasks;

namespace ERP_sys.Repositories
{
    public interface ICustomerLedgerRepository
    {
        Task<CustomerLedgerResponseDto?> GetCustomerLedgerAsync(CustomerLedgerFilterDto filter);
    }
}