using Microsoft.IdentityModel.Tokens;
using ProtectWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProtectWebApi.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Login),
                new(ClaimTypes.Role, "Admin"), // можно получать из данных пользователя
            };

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.Now.AddMinutes(30)
                //, issuer: "", audience: ""
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
