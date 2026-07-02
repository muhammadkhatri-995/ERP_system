using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ERP_sys.Models;
namespace ERP_sys.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _connectionString;

        public CustomerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. Create a new customer
        public async Task<int> CreateAsync(Customers customer)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                INSERT INTO Customers
                    (Name, Email, Phone, Address, City, CreatedBy, CreatedDate, IsDeleted)
                OUTPUT INSERTED.Id
                VALUES
                    (@Name, @Email, @Phone, @Address, @City, @CreatedBy, @CreatedDate, 0)",
                connection);

            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object)customer.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", customer.CreatedBy);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

            return (int)await cmd.ExecuteScalarAsync();
        }

        // 2. Get a customer by ID

        public async Task<Customers> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT * FROM Customers
                WHERE Id = @Id AND IsDeleted = 0",
                connection);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Customers
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"] as string,
                    Phone = reader["Phone"] as string,
                    Address = reader["Address"] as string,
                    City = reader["City"] as string,
                    CreatedBy = (int)reader["CreatedBy"],
                    CreatedDate = (DateTime)reader["CreatedDate"]
                }


              ;
            }

            return null;
        }
        // 3. Get all customers

        public async Task<List<Customers>> GetAllAsync()
        {
            var customers = new List<Customers>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT * FROM Customers
                WHERE IsDeleted = 0
                ORDER BY Name",
                connection);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new Customers
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"] as string,
                    Phone = reader["Phone"] as string,
                    City = reader["City"] as string,
                    Address = reader["Address"] as string
                });
            }

            return customers;
        }

        // 4. Update a customer

        public async Task<int> UpdateAsync(Customers customer)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                UPDATE Customers
                SET
                    Name = @Name,
                    Email = @Email,
                    Phone = @Phone,
                    Address = @Address,
                    City = @City
                WHERE Id = @Id AND IsDeleted = 0",
                connection);

            cmd.Parameters.AddWithValue("@Id", customer.Id);
            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Phone", (object)customer.Phone ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City", (object)customer.City ?? DBNull.Value);

            return await cmd.ExecuteNonQueryAsync();
        }


        public async Task<int> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new SqlCommand(@"
                UPDATE Customers
                SET IsDeleted = 1
                WHERE Id = @Id",
                connection);

            cmd.Parameters.AddWithValue("@Id", id);

            return await cmd.ExecuteNonQueryAsync();
        }
    }
}

    
