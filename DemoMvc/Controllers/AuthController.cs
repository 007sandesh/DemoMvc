using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository authRepo, IConfiguration config)
        {
            _authRepo = authRepo;
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Check if username already exists
            var existingUser = await _authRepo.GetUserByUsernameAsync(request.Username);
            if (existingUser != null)
                return BadRequest("Username already exists");

            // 2. Check if email already exists
            var existingEmail = await _authRepo.GetUserByEmailAsync(request.Email);
            if (existingEmail != null)
                return BadRequest("Email already exists");

            // 3. Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = request.Role  // "Admin", "Teacher", "Student"
            };

            await _authRepo.RegisterUserAsync(user, request.Password);

            return Ok(new { message = "User registered successfully" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Find user by username
            var user = await _authRepo.GetUserByUsernameAsync(request.Username);
            if (user == null)
                return Unauthorized("Invalid username or password");

            // 2. Validate password
            var isValidPassword = await _authRepo.ValidatePasswordAsync(user, request.Password);
            if (!isValidPassword)
                return Unauthorized("Invalid username or password");

            // 3. Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token = token,
                username = user.Username,
                role = user.Role
            });
        }

        // Helper method to generate JWT token
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // Request models
    // Request models
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }  // "Admin", "Teacher", "Student"
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}