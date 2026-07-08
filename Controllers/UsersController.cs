using ERP_sys.Attributes;
using ERP_sys.Models;
using ERP_sys.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ERP_sys.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _repository;

        public UsersController(UserRepository repository)
        {
            _repository = repository;
        }

        // 1. GET: api/roles

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _repository.GetRolesAsync();
            return Ok(roles);
        }

        // 2. GET: api/departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _repository.GetDepartmentsAsync();
            return Ok(departments);
        }

        // 3. GET: api/designations
        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations()
        {
            var designations = await _repository.GetDesignationsAsync();
            return Ok(designations);
        }

        // 4. POST: api/users (Create)
        [HttpPost("users")]
        [AuditAction("User created")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(new { message = "Username and Email are required fields." });
            }

            try
            {
                int newId = await _repository.CreateUserAsync(user);
                user.Id = newId;
                return Created($"/api/users/{newId}", user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database Error: " + ex.Message });
            }
        }

        // 5. GET: api/users (List all, with joined Role/Department/Designation names)
        [HttpGet("users")]
        [AuditAction("User list")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _repository.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database Error: " + ex.Message });
            }
        }

        // 6. PUT: api/users/{id} (Update)
        [HttpPut("users/{id}")]
        [AuditAction("checks user by id")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email))
            {
                return BadRequest(new { message = "Username and Email are required fields." });
            }

            user.Id = id;

            try
            {
                bool success = await _repository.UpdateUserAsync(user);
                if (!success)
                {
                    return NotFound(new { message = $"User with Id {id} not found." });
                }
                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database Error: " + ex.Message });
            }
        }

        // 7. DELETE: api/users/{id}
        [HttpDelete("users/{id}")]
        [AuditAction("delete user by id")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                bool success = await _repository.DeleteUserAsync(id);
                if (!success)
                {
                    return NotFound(new { message = $"User with Id {id} not found." });
                }
                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Database Error: " + ex.Message });
            }
        }
    }
}