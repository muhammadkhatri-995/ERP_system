using Microsoft.Data.SqlClient;
using System.Data;
using ERP_sys.Data;
using ERP_sys.Models;

namespace ERP_sys.Repositories
{
    public class UserRepository
    {
        private readonly SqlConnectionFactory _connectionFactory;

        public UserRepository(SqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // 1. Fetch Roles
        public async Task<List<Role>> GetRolesAsync()
        {
            var list = new List<Role>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_GetRoles", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            // id or role jab tak khatam nhi hote tab tk read krna he is lie while loop use kiya he
            while (await reader.ReadAsync())
            {
                list.Add(new Role
                {
                    Id = reader.GetInt32(0),
                    RoleName = reader.GetString(1)
                });
            }
            return list;
        }

        // 2. Fetch Departments
        public async Task<List<department>> GetDepartmentsAsync()
        {
            var list = new List<department>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_GetDepartments", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new department
                {
                    departmentId = reader.GetInt32(0),
                    departmentName = reader.GetString(1)
                });
            }
            return list;
        }

        // 3. Fetch Designations
        public async Task<List<designation>> GetDesignationsAsync()
        {
            var list = new List<designation>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_GetDesignations", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new designation
                {
                    Id = reader.GetInt32(0),
                    designationName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1)
                });
            }
            return list;
        }

        // 4. Create User
        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_CreateUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(user.Name) ? (object)DBNull.Value : user.Name;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = string.IsNullOrEmpty(user.Email) ? (object)DBNull.Value : user.Email;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, -1).Value = string.IsNullOrEmpty(user.PasswordHash) ? "" : user.PasswordHash;
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@RoleId", user.RoleId);
            command.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
            command.Parameters.AddWithValue("@DesignationId", user.DesignationId);

            return (int)await command.ExecuteScalarAsync();
        }

        // 5. Get All Users With Joins
        public async Task<List<User>> GetUsersAsync()
        {
            var list = new List<User>();
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_GetUsers", connection);
            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    IsActive = reader.GetBoolean(3),
                    RoleId = reader.GetInt32(4),
                    roleName = reader.GetString(5),
                    DepartmentId = reader.GetInt32(6),
                    departmentName = reader.GetString(7),
                    DesignationId = reader.GetInt32(8),
                    designationName = reader.GetString(9)
                });
            }
            return list;
        }

        // 6. Update User
        public async Task<bool> UpdateUserAsync(User user)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_UpdateUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = string.IsNullOrEmpty(user.Name) ? (object)DBNull.Value : user.Name;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = string.IsNullOrEmpty(user.Email) ? (object)DBNull.Value : user.Email;
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@RoleId", user.RoleId);
            command.Parameters.AddWithValue("@DepartmentId", user.DepartmentId);
            command.Parameters.AddWithValue("@DesignationId", user.DesignationId);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        // 7. Delete User
        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = await _connectionFactory.CreateConnectionAsync();
            using var command = new SqlCommand("sp_DeleteUser", connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }
    }
}