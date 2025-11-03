using Microsoft.IdentityModel.Tokens;
using MoviesStore.models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoviesStore.Services
{
    public class JwtService(IConfiguration config) : IJwtService
    {
        private readonly IConfiguration _config = config;

        public string GenerateToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new[]
               {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("uid", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(
               issuer: jwt["Issuer"],
               audience: jwt["Audience"],
               claims: claims,
               expires: DateTime.UtcNow.AddHours(6),
               signingCredentials: creds
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
