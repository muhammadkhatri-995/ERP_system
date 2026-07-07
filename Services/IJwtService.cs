using ERP_sys.Models;
namespace ERP_sys.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}