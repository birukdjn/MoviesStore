using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesStore.data;
using MoviesStore.models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly AppDbContext _Context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _Context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDto request)
        {
            if (_Context.Users.Any(u => u.Username == request.username))
                return BadRequest("Username already exists.");

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.password);

            var user = new User
            {
                Username = request.username,
                PasswordHash = PasswordHash,
            };

            _Context.Users.Add(user);
            _Context.SaveChanges();

            return Ok("User registered Successfully!");
        }

        [HttpPost("login")]

        public IActionResult Login(UserDto request)
        {
            var user = _Context.Users.FirstOrDefault(u => u.Username == request.username);
            if (user == null)
              return BadRequest($"user {request.username} not found");

            if (!BCrypt.Net.BCrypt.Verify(request.password, user.PasswordHash))

                return BadRequest("Wrong Password");

            string token = CreateToken(user);
            return Ok(new { Token = token });



        }

        private string CreateToken(User user)
        {
            List<Claim> Claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:key")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: Claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


    }

    public class UserDto
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}
