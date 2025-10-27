using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MoviesStore.data;
using MoviesStore.models;

namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RatingsController:ControllerBase
    {
        private readonly AppDbContext _context;
        public RatingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RateMovie(int movieId, int score)
        {
            if(score <1 || score>5)
               return BadRequest("Score must be between 1 and 5.");

            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if(user == null)
                return Unauthorized();

            var existing = _context.Ratings.FirstOrDefault(r => r.UserId ==user.Id && r.MovieId==movieId);

            if(existing != null) {
                existing.Score=score;

            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    UserId = user.Id,
                    MovieId = movieId,
                    Score = score
                });
            }
            _context.SaveChanges();
            return Ok("Rating saved!");

        }

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
