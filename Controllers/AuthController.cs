using ERP_sys.Models.DTOs;
using ERP_sys.Repositories;
using ERP_sys.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ERP_sys.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Email and password are required" });

            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid Email or Password." });

            if (!user.IsActive)
                return Unauthorized(new { message = "This account has been deactivated." });

            bool passwordValid = request.Password == user.PasswordHash;
            if (!passwordValid)
                return Unauthorized(new { message = "Invalid Email or Password." });

            var token = _jwtService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.IsAdmin ? "Admin" : "User"
            });
        }
    }
}