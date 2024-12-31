using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SurpassIntegration.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurpassIntegration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // 1. Validate user credentials (persistent store)
            var user = _userService.ValidateUser(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            // 2. Generate JWT
            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                // Sub = subject of the JWT, typically unique identifier
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                // Jti = JWT ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                // Additional custom claim for Surpass reference
                new Claim("candidateReference", request.Username)
            };

            var token = new JwtSecurityToken(
                issuer: null,   // configure appropriately in production
                audience: null, // configure appropriately in production
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // 3. Return token to client
            return Ok(new { token = tokenString });
        }
    }

    public record LoginRequest(string Username, string Password);
}
