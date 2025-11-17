using Backend.data;
using Backend.DTOs;
using Backend.models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AppDbContext context, IJwtService jwt, IEmailSender emailSender) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IJwtService _jwt = jwt;
        private readonly IEmailSender _emailSender = emailSender;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest("Username or Email already exists.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Avatar = dto.Avatar ??  "default_avatar.png",
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var defaultProfile = new Profile
            {
                UserId = user.Id,
                Name = user.Username,
                Avatar = user.Avatar ?? "default_avatar.png",
                IsKidsProfile = false
            };

            _context.Profiles.Add(defaultProfile);
            await _context.SaveChangesAsync();

            try
            {
                var welcomeMessage = new Message(
                    [user.Email],
                    "Welcome to Our Streaming Service!",
                    $"<h1>Hello {user.Username},</h1><p>Thank you for registering! You can now log in and start watching on your default profile.</p><p>If you have any questions, feel free to contact us.</p>"
                   
                );

                // 🚀 Call the SendEmail method
                _emailSender.SendEmail(welcomeMessage);
            }
            catch (Exception ex)
            {
                // In a production application, you should log this error, 
                // but registration success should NOT depend on email success.
                Console.WriteLine($"Error sending welcome email to {user.Email}: {ex.Message}");
            }

            var userToken = _jwt.GenerateUserToken(user);
            defaultProfile.User = user;
            var profileToken = _jwt.GenerateProfileToken(defaultProfile);
            var refreshToken = _jwt.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered and default profile created successfully.",
                userId = user.Id,
                username = user.Username,
                userToken,
                profileToken,
                refreshToken,
                defaultProfile = new
                {
                    id = defaultProfile.Id,
                    name = defaultProfile.Name,
                    defaultProfile.Avatar,
                }
            });
        }      


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.LoginIdentifier || u.Username == dto.LoginIdentifier);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });

            var currentIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(currentIp) && user.LastLoginIp != currentIp)
            {
                try
                {
                    var securityAlertMessage = new Message(
                        [user.Email],
                        "Security Alert: New Login Location Detected",
                        $"<p>A login was just detected for your account from a new IP address: <strong>{currentIp}</strong>.</p><p>If this was you, you can safely ignore this email. If this was not you, please change your password immediately.</p>"
                        

                    );
                    _emailSender.SendEmail(securityAlertMessage);

                    
                    // Update the user's last login IP
                    user.LastLoginIp = currentIp;
                    await _context.SaveChangesAsync(); // Save the IP change
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending security alert email to {user.Email}: {ex.Message}");
                }
            }

            var userToken = _jwt.GenerateUserToken(user);

            // ✅ Separate logic for Admin
            if (user.Role == "Admin")
            {
                var refreshToken = _jwt.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    token = userToken,
                    refreshToken,
                    role = user.Role
                });
            }

            // ✅ Regular user flow (with profiles)
            var profiles = await _context.Profiles
                .Where(p => p.UserId == user.Id)
                .Select(p => new { p.Id, p.Name, p.Avatar })
                .ToListAsync();

            var refresh = _jwt.GenerateRefreshToken();
            user.RefreshToken = refresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = userToken,
                profiles,
                refreshToken = refresh,
                role = user.Role
            });
        }


        private int GetCurrentUserId()
        {
            var userClaimId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userClaimId, out int userId) ? userId : 0;
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto dto)
        {
            int userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "Invalid or missing user ID in token." });

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            bool changesMade = false;

            if (!string.IsNullOrEmpty(dto.Username) && user.Username != dto.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != userId))
                    return BadRequest("New username is already taken.");

                user.Username = dto.Username;
                changesMade = true;
            }

            if (!string.IsNullOrEmpty(dto.Email) && user.Email != dto.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != userId))
                    return BadRequest("New email is already taken.");

                user.Email = dto.Email;
                changesMade = true;
            }

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                if (string.IsNullOrEmpty(dto.CurrentPassword))
                    return BadRequest("Current password is required to change password.");

                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                    return Unauthorized(new { message = "Incorrect current password." });

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                changesMade = true;
            }

            if (changesMade)
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully." });
            }

            return Ok(new { message = "No changes submitted." });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserPublicDto>>> GetRegisteredUsers()
        {
            var users = await _context.Users
                .Select(u => new UserPublicDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();

            if (users.Count==0) return NotFound("No users found.");

            return Ok(users);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public IActionResult Refresh([FromBody] Backend.models.RefreshRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            var newToken = _jwt.GenerateUserToken(user);
            var newRefreshToken = _jwt.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            _context.SaveChanges();

            return Ok(new { token = newToken, refreshToken = newRefreshToken });
        }
    }
}
