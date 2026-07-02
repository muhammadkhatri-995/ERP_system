using System.Collections.Generic;
using System.Threading.Tasks;
using ERP_sys.Models;

namespace ERP_sys.Repositories
{
    public interface ICustomerRepository
    {
        Task<int> CreateAsync(Customers customer);
        Task<Customers> GetByIdAsync(int id);
        Task<List<Customers>> GetAllAsync();
        Task<int> UpdateAsync(Customers customer);
        Task<int> DeleteAsync(int id);


    }
}