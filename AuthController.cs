using CozyComfort.API.Data;
using CozyComfort.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CozyComfort.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email and Password are required");

            
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest("User with this email already exists");

            
            if (request.Role != "Seller" && request.Role != "Distributor" && request.Role != "Client")
            {
                request.Role = "Client"; 
            }

            var hashedPassword = HashPassword(request.Password);

            var newUser = new AppUser
            {
                Email = request.Email, 
                PasswordHash = hashedPassword,
                Role = request.Role
            };

            
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Registration successful as {newUser.Role}", Email = newUser.Email, Role = newUser.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Find user in DATABASE (Using Email now)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { Success = false, Message = "Invalid email or password" });

            // Verify password
            if (user.PasswordHash != HashPassword(request.Password))
                return Unauthorized(new { Success = false, Message = "Invalid email or password" });

            // Return success with Role
            return Ok(new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Email = user.Email, // Returning Email
                Role = user.Role
            });
        }

        // Helper method to hash passwords
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}