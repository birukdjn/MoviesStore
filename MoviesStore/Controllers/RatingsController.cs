using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesStore.data;
using MoviesStore.models;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RatingsController(AppDbContext context)
        {
            _context = context;
        }

        // Rate a movie
        [HttpPost("{movieId}")]
        public IActionResult RateMovie(int movieId, int score)
        {
            if (score < 1 || score > 5)
                return BadRequest("Score must be between 1 and 5.");

            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return Unauthorized();

            // Get the first profile of the user
            var profile = _context.Profiles.FirstOrDefault(p => p.UserId == user.Id);
            if (profile == null)
                return BadRequest("Profile not found");

            // Check if rating already exists
            var existing = _context.Ratings.FirstOrDefault(r => r.ProfileId == profile.Id && r.MovieId == movieId);

            if (existing != null)
            {
                existing.Score = score; // Update existing rating
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    ProfileId = profile.Id,
                    MovieId = movieId,
                    Score = score
                });
            }

            _context.SaveChanges();
            return Ok("Rating saved!");
        }

        // Get average rating of a movie
        [HttpGet("{movieId}/average")]
        public IActionResult GetAverageRating(int movieId)
        {
            var ratings = _context.Ratings.Where(r => r.MovieId == movieId).ToList();
            if (!ratings.Any()) return Ok(0);

            double average = ratings.Average(r => r.Score);
            return Ok(average);
        }
    }
}
