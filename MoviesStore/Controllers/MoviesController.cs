using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using MoviesStore.DTOs;
using MoviesStore.models;

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

        // GET: All movies
        [HttpGet]
        [Authorize(Roles ="admin,user")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            if (!movies.Any())
                return NotFound("No movies found.");
            return Ok(movies);
        }

        // GET: Search movies by keyword
        [HttpGet("search")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> SearchMovies(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var movies = await _context.Movies
                .Include(m => m.MovieGenres!)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories!)
                    .ThenInclude(mc => mc.Category)
                .Where(m =>
                    m.Title.Contains(query) ||
                    m.Director.Contains(query) ||
                    m.MovieGenres.Any(mg => mg.Genre.Name.Contains(query)) ||
                    m.MovieCategories.Any(mc => mc.Category.Name.Contains(query))
                    ).ToListAsync();

            return Ok(movies);
        }

        //GET: Paged movies
        [HttpGet("paged")]
        [Authorize(Roles = "admin,user")]
        public IActionResult GetPagedMovies(int page = 1, int pageSize = 10)
        {
            var movies = _context.Movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(movies);
        }

        // GET: Filter movies by genre, year, or director
        [HttpGet("filter")]
        [Authorize(Roles = "admin,user")]
        public IActionResult FilterMovies(int? genreId, int? categoryId,  int? year, string? director)
        {
            var query = _context.Movies.AsQueryable();

            if (genreId.HasValue)
                query = query.Where(m => m.MovieGenres.Any(mg =>mg.GenreId == genreId.Value));
            
            if (categoryId.HasValue)
                query = query.Where(m => m.MovieCategories.Any(mc => mc.CategoryId == categoryId.Value));

            if (year.HasValue)
                query = query.Where(m => m.ReleaseYear == year.Value);

            if (!string.IsNullOrEmpty(director))
                query = query.Where(m => m.Director.Contains(director));

            var filteredMovies = query
                .Include(m => m.MovieGenres!)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieCategories!)
                    .ThenInclude(mc => mc.Category)
                .ToList();

            return Ok(filteredMovies);

        }

        // GET: Single movie by ID
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return NotFound($"Movie with id {id} not found.");
            return Ok(movie);
        }

        // GET: Recommend similar movies
        [HttpGet("recommend/{movieId}")]
        [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> RecommendMovies(int movieId)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == movieId);
            if (movie == null)
                return NotFound("Movie not found.");

            var sharedCategoryIds = movie.MovieCategories.Select(mc => mc.CategoryId).ToList();

            var recommended = await _context.Movies
                .Where(m => m.Id != movieId &&
                            m.MovieCategories.Any(mc => sharedCategoryIds.Contains(mc.CategoryId)))
                            .Take(5)
                            .ToListAsync();

                            return Ok(recommended);
        }

        // POST: Add a new movie
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Movie>> AddMovie([FromBody] MovieCreateDto dto)
        {
            

            var movie = new Movie
            {
                Title = dto.Title,
                ReleaseYear = dto.ReleaseYear,
                Director = dto.Director,
                Description = dto.Description,
            };

            foreach (var categoryId in dto.CategoryIds)
            {
                var categiroy = await _context.Categories.FindAsync(categoryId);
                if (categiroy ==null)
                    return BadRequest($"Invalid category ID: {categoryId}.");

                movie.MovieCategories.Add(new MovieCategory { CategoryId = categoryId });

            }
            foreach (var genreId in dto.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre == null)
                    return BadRequest($"Invalid genre ID: {genreId}.");

                movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });
            }

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // ✅ PUT: Update a movie
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieCreateDto dto)
        {
            int currentYear = DateTime.Now.Year;

            var existing = await _context.Movies
                .Include(m => m.MovieGenres)
                .Include(m => m.MovieCategories)
                .FirstOrDefaultAsync(m => m.Id == id);

            
            if (existing == null)
                return NotFound($"Movie with id {id} not found.");

            if (dto.ReleaseYear > currentYear || dto.ReleaseYear < 1900)
                return BadRequest("Release year must be between 1900 and the current year.");

            
            existing.Title = dto.Title;
            existing.ReleaseYear = dto.ReleaseYear;
            existing.Director = dto.Director;
            existing.Description = dto.Description;

            existing.MovieGenres.Clear();

            foreach (var genreId in dto.GenreIds)
            {
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == genreId);
                if (!genreExists)
                    return BadRequest($"Invalid genre ID: {genreId}.");

                existing.MovieGenres.Add(new MovieGenre { MovieId = id, GenreId = genreId });
            }

            existing.MovieCategories.Clear();
            foreach (var categoryId in dto.CategoryIds)
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                    return BadRequest($"Invalid category ID: {categoryId}.");

                existing.MovieCategories.Add(new MovieCategory { MovieId = id, CategoryId = categoryId });
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ✅ DELETE: Delete a movie
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]

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
