using ERP_sys.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ERP_sys.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Products>> GetAllProductsAsync()
        {
            var products = new List<Products>();

            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"SELECT
                                Id,
                                Name,
                                SKU,
                                Category,
                                Price,
                                StockQuantity,
                                Unit
                             FROM Products";

            using SqlCommand command = new SqlCommand(query, connection);

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Products
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    SKU = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Price = reader.GetDecimal(4),
                    StockQuantity = reader.GetDecimal(5),
                    Unit = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }

            return products;
        }
    }
}