using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SecureVault.API.Data;
using SecureVault.API.Models;
using SecureVault.Shared.DTOs;

namespace SecureVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SecureVaultDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(SecureVaultDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// The provided password is hashed with BCrypt before being 
        /// persisted; the plain-text password is never stored.
        /// </summary>
        /// <param name="dto">The data transfer object containing the
        /// username, email, and password of the account to create.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the newly created UserDto, or a
        /// 400 Bad Request response if the email is already in use.</returns>
        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Authenticates a user with an email and password.
        /// On success, a signed JWT is generated and returned, which
        /// must be included as a Bearer token is subsequent request
        /// to protected endpoints.
        /// </summary>
        /// <param name="dto">The data transfer object containing the
        /// email and password to authenticate with</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the generated JWT, or a 401
        /// Unauthorized response if the credentials are invalid.</returns>
        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Generates a signed JWT for the specified user, containing
        /// their ID, username, email and role as claims.
        /// The token is valid for 15 min.
        /// </summary>
        /// <param name="user">The user for whom the token should
        /// be generated.</param>
        /// <returns>A signed JWT as a string.</returns>
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}