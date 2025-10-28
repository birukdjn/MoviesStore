using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesStore.Data;
using MoviesStore.models;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateProfile(string profileName)
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var profile = new Profile
            {
                Name = profileName,
                UserId = user.Id
            };

            _context.Profiles.Add(profile);
            _context.SaveChanges();

            return Ok(profile);
        }

        [HttpGet]
        public IActionResult GetProfiles()
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var profiles = _context.Profiles.Where(p => p.UserId == user.Id).ToList();
            return Ok(profiles);
        }
    }
}
