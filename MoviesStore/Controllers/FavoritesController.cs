using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using MoviesStore.models;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // Add a movie to favorites
        [HttpPost("{movieId}")]
        [Authorize(Roles ="Admin,User")]
        public IActionResult AddToFavorites(int movieId)
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            // Get the first profile of the user 
            var profile = _context.Profiles.FirstOrDefault(p => p.UserId == user.Id);
            if (profile == null) return BadRequest("Profile not found");

            // Check if already favorited
            if (_context.Favorites.Any(f => f.ProfileId == profile.Id && f.MovieId == movieId))
                return BadRequest("Movie already in favorites.");

            var favorite = new Favorite
            {
                ProfileId = profile.Id,
                MovieId = movieId
            };

            _context.Favorites.Add(favorite);
            _context.SaveChanges();

            return Ok("Added to favorites");
        }

        // Get all favorites for the current user's profile
        [HttpGet]
        [Authorize(Roles = "Admin,User")]

        public IActionResult GetFavorites()
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var profile = _context.Profiles.FirstOrDefault(p => p.UserId == user.Id);
            if (profile == null) return BadRequest("Profile not found");

            var favorites = _context.Favorites
                .Where(f => f.ProfileId == profile.Id)
                .Include(f => f.Movie)
                .Select(f => f.Movie)
                .ToList();

            return Ok(favorites);
        }

        // Remove a movie from favorites
        [HttpDelete("{movieId}")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult RemoveFavorite(int movieId)
        {
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return Unauthorized();

            var profile = _context.Profiles.FirstOrDefault(p => p.UserId == user.Id);
            if (profile == null) return BadRequest("Profile not found");

            var favorite = _context.Favorites
                .FirstOrDefault(f => f.ProfileId == profile.Id && f.MovieId == movieId);
            if (favorite == null) return NotFound("Favorite not found.");

            _context.Favorites.Remove(favorite);
            _context.SaveChanges();

            return Ok("Removed from favorites");
        }
    }
}
