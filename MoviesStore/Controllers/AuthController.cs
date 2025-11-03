using Microsoft.AspNetCore.Mvc;
using MoviesStore.models;
using MoviesStore.Data;
using MoviesStore.Services;
using MoviesStore.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MoviesStore.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext context, IJwtService jwt) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            
            if (await context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists.");

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = PasswordHash,
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            
            var user = await context.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);

            
            if ((user == null) || (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)))
                return Unauthorized(new { message = "Invalid credentials" });

            
            var token = jwt.GenerateToken(user);

            return Ok(new { token }); 
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserPublicDto>>> GetRegisteredUsers()
        {
            
            var users = await context.Users
                .Select(u => new UserPublicDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role 
                })
                .ToListAsync();

            if (!users.Any())
            {
                return NotFound("No users found.");
            }

            return Ok(users); 
        }
    }



}