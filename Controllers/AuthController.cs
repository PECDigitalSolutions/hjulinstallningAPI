using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HjulinstallningAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        public static string _cachedToken = string.Empty;  // Store the token in memory
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "password")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing in config"));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, request.Username) }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                _cachedToken = jwtToken;  // 🔹 Store the token in memory

                return Ok(new { token = jwtToken });
            }

            return Unauthorized();
        }

        [HttpGet("token")]
        public IActionResult GetToken()
        {
            if (string.IsNullOrEmpty(_cachedToken))
            {
                return Unauthorized(new { message = "No token found. Please log in first." });
            }

            return Ok(new { token = _cachedToken });
        }
    }
}



    public class LoginRequest
    {
        private string _username = string.Empty;  // Default value
        private string _password = string.Empty;  // Default value

        public string Username
        {
            get => _username;
            set => _username = value ?? throw new ArgumentNullException(nameof(Username), "Username cannot be null or empty.");
        }

        public string Password
        {
            get => _password;
            set => _password = value ?? throw new ArgumentNullException(nameof(Password), "Password cannot be null or empty.");
        }
    }


