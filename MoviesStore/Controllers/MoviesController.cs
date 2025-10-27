using Microsoft.AspNetCore.Mvc;
using MoviesStore.data;
using MoviesStore.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace MovieStoreApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MoviesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }

        [HttpGet("search")]
        public IActionResult SearchMovies(string query)
        {
            if(string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var movies = _context.Movies
                .Where(m => m.Title.Contains(query)||
                            m.Genre.Contains(query)||
                            m.Director.Contains(query))
                            .ToList();
            return Ok(movies);

        }

        [HttpGet("paged")]
        public IActionResult GetPagedMovies(int page = 1, int pageSize = 10)
        {
            var movies = _context.Movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(movies);
        }

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



        [HttpGet("{id}")]
        public ActionResult<Movie> GetMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
                return NotFound();
            return Ok(movie);
        }

        [HttpPost]
        public ActionResult<Movie> AddMovie(Movie movie)
        {
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, Movie movie)
        {
            var existing = _context.Movies.Find(id);
            if (existing == null)
                return NotFound();

            existing.Title = movie.Title;
            existing.Genre = movie.Genre;
            existing.ReleaseYear = movie.ReleaseYear;
            existing.Director = movie.Director;
            existing.Description = movie.Description;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null)
                return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
