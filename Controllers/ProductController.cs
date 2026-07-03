using ERP_sys.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ERP_sys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllProductsAsync();
            return Ok(products);
        }
    }
}