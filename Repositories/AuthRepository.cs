using ERP_sys.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
namespace ERP_sys.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Users WHERE Email = @Email";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString()!,
                                Email = reader["Email"].ToString()!,
                                PasswordHash = reader["PasswordHash"].ToString()!,
                                IsActive = (bool)reader["IsActive"],
                                IsAdmin = (bool)reader["IsAdmin"],
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                    }
                }
            }
            return null; // User not found
        }
    }
}