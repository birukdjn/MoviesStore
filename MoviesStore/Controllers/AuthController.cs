using Microsoft.AspNetCore.Mvc;
using MoviesStore.models;
using MoviesStore.Data;
using MoviesStore.Services;
using MoviesStore.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.VisualBasic;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext context, IJwtService jwt) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {

            if (await context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest("Username or Email already exists.");

            string PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = PasswordHash,
                Role = "User",
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var defaultProfile = new Profile
            {
                UserId = user.Id,
                Name = user.Username,
                AvatarUrl = "default_avatar.png",
                IsKidsProfile = false
            };

            context.Profiles.Add(defaultProfile);
            await context.SaveChangesAsync();

            var userToken = jwt.GenerateUserToken(user);
            defaultProfile.User = user;

            var profileToken = jwt.GenerateProfileToken(defaultProfile);

            return Ok(new
            {
                message = "User registered and default profile created successfully.",
                userId = user.Id,
                username = user.Username,
                userToken,
                profileToken,
                defaultProfile = new {
                    id = defaultProfile.Id,
                    name = defaultProfile.Name
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await context.Users
                   .FirstOrDefaultAsync(u => u.Email == dto.LoginIdentifier || u.Username == dto.LoginIdentifier);

            if ((user == null) || (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)))
                return Unauthorized(new { message = "Invalid credentials" });

            var userToken = jwt.GenerateUserToken(user);
            var profile = await context.Profiles
                .Where(p => p.UserId == user.Id)
                .Select(p => new { p.Id, p.Name, p.AvatarUrl })
                .ToListAsync();

            return Ok(new {
                userToken,
                profile
            });
        }


        private int GetCurrentUserId()
        {
            var userClaimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userClaimId, out int userId)) return userId;
            { return userId; }
            return 0;
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            int userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized(new { message = "Invalid or missing user ID in token." });

            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            bool changesMade = false;

            if (!string.IsNullOrEmpty(dto.Username) && user.Username != dto.Username)
            {
                if (await context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != userId))
                {
                    return BadRequest("New username is already taken.");
                }
                user.Username = dto.Username;
                changesMade = true;
            }

            if (!string.IsNullOrEmpty(dto.Email) && user.Email != dto.Email)
            {
                if (await context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                {
                    return BadRequest("New email is already taken.");
                }
                user.Email = dto.Email;
                changesMade = true;
            }

            if (!string.IsNullOrEmpty(dto.NewPassword) && !string.IsNullOrEmpty(dto.CurrentPassword))
            {
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                {
                    return Unauthorized(new { message = "Incorrect current password." });
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                changesMade = true;
            }

            else if (!string.IsNullOrEmpty(dto.NewPassword) && string.IsNullOrEmpty(dto.CurrentPassword))
            {
                return BadRequest("Current password is required to change password.");
            }

            if (changesMade)
            {
                await context.SaveChangesAsync();
                return Ok(new
                {
                    message = "User information updated successfully.",
                    username = user.Username,
                    email = user.Email
                });
            }

            return Ok(new { message = "No changes were submitted." });
        }
    


        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserPublicDto>>> GetRegisteredUsers()
        {
            var users = await context.Users
                .Select(u => new UserPublicDto
                {
                    Id = u.Id,
                    Email = u.Email,
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