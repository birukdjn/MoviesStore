using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using MoviesStore.data;
using MoviesStore.models;

namespace MoviesStore.Controllers
{
   
        [Route("api/[controller]")]
        [ApiController]
        [Authorize]

        public class FavoritesController : ControllerBase
        {
            private readonly AppDbContext _context;

            public FavoritesController(AppDbContext context)
            {
                _context = context;
            }

            [HttpPost("movieId")]
            public IActionResult AddToFavorites(int movieId)
            {
                var username = User.Identity?.Name;
                var user = _context.Users.FirstOrDefault(u => u.Username == username);
                if (user == null)
                    return Unauthorized();

                if (_context.Favorites.Any(f => f.UserId == user.Id && f.MovieId == movieId))
                    return BadRequest("Movie is already in favorites.");

                var favorite = new Favorite
                {
                    UserId = user.Id,
                    MovieId = movieId
                };
                _context.Favorites.Add(favorite);
                _context.SaveChanges();

                return Ok("Added to favorites");
            }

            [HttpGet]
            public IActionResult GetFavorites()
            {
                var username = User.Identity?.Name;
                var user = _context.Users.Include(u => u.Id).FirstOrDefault(u=>u.Username == username);

                if (user == null) return Unauthorized();

                var favorites = _context.Favorites
                    .Where(f => f.UserId == user.Id)
                    .Include(f => f.Movie)
                    .Select(f => f.Movie)
                    .ToList();

                return Ok(favorites);
            }
            [HttpDelete("{movieId}")]
            public IActionResult RemoveFavorite(int MovieId)
            {
                var username = User.Identity?.Name;
                var user = _context.Users.FirstOrDefault(u => u.Username == username);

                if (user ==null) return Unauthorized();

                var favorite = _context.Favorites.FirstOrDefault(f => f.UserId==user.Id && f.MovieId==MovieId);
                if(favorite == null) return NotFound("Favorite not found.");
                _context.Favorites.Remove(favorite);
                _context.SaveChanges();

                return Ok("Removed from favorites.");


            }


        }
    }
