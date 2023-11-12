using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthDemo.Models;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtConfig _jwtConfig;

        public AuthController(JwtConfig jwtConfig)
        {
            _jwtConfig = jwtConfig;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            // Ovdje bi bila logika za provjeru korisnika 
            if (loginRequest.Email == "davor@gmail.com" && loginRequest.Password == "Davor123!")
            {
                var token = GenerateJwtToken(loginRequest.Email, "Davor", "User");
                return Ok(new { token });
            }
            return Unauthorized("Neispravni email ili lozinka.");
        }

        [HttpPost("update-user")]
        public IActionResult UpdateUser(UpdateUserRequest updateUserRequest)
        {
            // Ovdje bi bila logika za provjeru tokena i ažuriranje korisnika
            if (updateUserRequest.Email != "davor@gmail.com")
            {
                return BadRequest("Neispravni email.");
            }

            var token = GenerateJwtToken(updateUserRequest.Email, updateUserRequest.Name, updateUserRequest.Role);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string email, string name, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



        [Authorize] // Ovaj atribut zahtjeva validan JWT token za pristup metodi
        [HttpPost("dummy-test")]
        public IActionResult DummyTest([FromBody] DummyTestRequest request)
        {
            // Logika ovdje ne radi ništa, osim što vraća status 200 OK.
            return Ok();
        }
    }
}
