using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using MoviesStore.models;
using MoviesStore.Models.DTOs;

namespace MoviesStore.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: All movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            if (!movies.Any())
                return NotFound("No movies found.");
            return Ok(movies);
        }

        // ✅ GET: Search movies by keyword
        [HttpGet("search")]
        public IActionResult SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var movies = _context.Movies
                .Where(m =>
                    m.Title.Contains(query) ||
                    m.Genre.Contains(query) ||
                    m.Director.Contains(query))
                .ToList();

            return Ok(movies);
        }

        // ✅ GET: Paged movies
        [HttpGet("paged")]
        public IActionResult GetPagedMovies(int page = 1, int pageSize = 10)
        {
            var movies = _context.Movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(movies);
        }

        // ✅ GET: Filter movies by genre, year, or director
        [HttpGet("filter")]
        public IActionResult FilterMovies(string? genre, int? year, string? director)
        {
            var query = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(m => m.Genre == genre);

            if (year.HasValue)
                query = query.Where(m => m.ReleaseYear == year.Value);

            if (!string.IsNullOrEmpty(director))
                query = query.Where(m => m.Director.Contains(director));

            return Ok(query.ToList());
        }

        // ✅ GET: Single movie by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");
            return Ok(movie);
        }

        // ✅ GET: Recommend similar movies
        [HttpGet("recommend/{movieId}")]
        public IActionResult RecommendMovies(int movieId)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie == null)
                return NotFound("Movie not found.");

            var recommended = _context.Movies
                .Where(m => m.CategoryId == movie.CategoryId && m.Id != movieId)
                .Take(5)
                .ToList();

            return Ok(recommended);
        }

        // ✅ POST: Add a new movie
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Movie>> AddMovie([FromBody] MovieCreateDto dto)
        {
            // Model validation handled automatically by [ApiController]

            var category = await _context.Categories.FindAsync(dto.CategoryId);
            if (category == null)
                return BadRequest("Invalid category.");

            var movie = new Movie
            {
                Title = dto.Title,
                Genre = dto.Genre,
                ReleaseYear = dto.ReleaseYear,
                Director = dto.Director,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // ✅ PUT: Update a movie
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieCreateDto movie)
        {
            int currentYear = DateTime.Now.Year;

            var existing = await _context.Movies.FindAsync(id);
            if (existing == null)
                return NotFound($"Movie with id {id} not found.");

            if (movie.ReleaseYear > currentYear || movie.ReleaseYear < 1900)
                return BadRequest("Release year must be between 1900 and the current year.");

            var categoryExists = _context.Categories.Any(c => c.Id == movie.CategoryId);
            if (!categoryExists)
                return BadRequest($"Category with id {movie.CategoryId} not found.");

            existing.Title = movie.Title;
            existing.Genre = movie.Genre;
            existing.ReleaseYear = movie.ReleaseYear;
            existing.Director = movie.Director;
            existing.Description = movie.Description;
            existing.CategoryId = movie.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: Delete a movie
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
