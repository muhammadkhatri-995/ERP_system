using ERP_sys.Models;
namespace ERP_sys.Repositories
{
	public interface IProductRepository
	{
		
	
		Task<List<Products>> GetAllProductsAsync();
		
	}
}