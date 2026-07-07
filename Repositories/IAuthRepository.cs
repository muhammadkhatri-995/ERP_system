using ERP_sys.Models;
using System.Threading.Tasks;

namespace ERP_sys.Repositories
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);

        }
}