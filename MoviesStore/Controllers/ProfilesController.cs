using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using MoviesStore.models;
using MoviesStore.DTOs;
using System.Security.Claims;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        
        public async Task<IActionResult> CreateProfile([FromBody] ProfileCreateDto dto)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return Unauthorized("Invalid or missing user information.");


            if (await _context.Profiles.AnyAsync(p => p.UserId == userId.Value))
            {
                
            }

            var profile = new Profile
            {
                FirstName = dto.FirstName, 
                LastName = dto.LastName,   
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Country = dto.Country,
                Address = dto.Address,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                DateOfBirth = dto.DateOfBirth,
                UserId = userId.Value
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profiles = await _context.Profiles
                .Where(p => p.UserId == userId.Value)
                .ToListAsync();

            if (!profiles.Any())
                return NotFound("No profiles found for this user.");

            return Ok(profiles);
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(int id)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (profile == null)
                return NotFound($"Profile with ID {id} not found or does not belong to the user.");

            return Ok(profile);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] ProfileCreateDto dto)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (existingProfile == null)
                return NotFound($"Profile with ID {id} not found or does not belong to the user.");

            
            existingProfile.FirstName = dto.FirstName;
            existingProfile.LastName = dto.LastName;
            existingProfile.Email = dto.Email;
            existingProfile.PhoneNumber = dto.PhoneNumber;
            existingProfile.Country = dto.Country;
            existingProfile.Address = dto.Address;
            existingProfile.City = dto.City;
            existingProfile.State = dto.State;
            existingProfile.ZipCode = dto.ZipCode;
            existingProfile.DateOfBirth = dto.DateOfBirth; 

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            int? userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId.Value);

            if (profile == null)
                return NotFound($"Profile with ID {id} not found or does not belong to the user.");

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}