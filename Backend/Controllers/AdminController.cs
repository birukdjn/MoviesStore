using Backend.data;
using Backend.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

            [HttpGet("stats")]
            public async Task<IActionResult> GetStats()
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalProfiles = await _context.Profiles.CountAsync();
                var totalMovies = await _context.Movies.CountAsync();
                var totalCategories = await _context.Categories.CountAsync();
                var totalGenres = await _context.Genres.CountAsync();
                var totalFavorites = await _context.Favorites.CountAsync();
                var totalRatings = await _context.Ratings.CountAsync();
                var averageRating = await _context.Ratings.AverageAsync(r => (double?)r.Score) ?? 0;
                var totalPlaybackPositions = await _context.PlaybackPositions.CountAsync();
                var totalSubscriptions = await _context.Subscriptions.CountAsync();

                // Fix for IsActive
                var activeSubscriptions = await _context.Subscriptions
                    .CountAsync(s => s.EndDate == null || s.EndDate > DateTime.UtcNow);

                var planDistribution = await _context.Subscriptions
                    .GroupBy(s => s.Plan)
                    .Select(g => new { Plan = g.Key.ToString(), Count = g.Count() })
                    .ToListAsync();

                var stats = new
                {
                    users = new
                    {
                        totalUsers,
                        totalProfiles
                    },
                    content = new
                    {
                        totalMovies,
                        totalCategories,
                        totalGenres
                    },
                    engagement = new
                    {
                        totalFavorites,
                        totalRatings,
                        averageRating,
                        totalPlaybackPositions
                    },
                    subscriptions = new
                    {
                        totalSubscriptions,
                        activeSubscriptions,
                        planDistribution
                    }
                };

                return Ok(stats);
            }

        }
    }

