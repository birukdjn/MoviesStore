using Backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.data;
using Backend.models;
using System.Security.Claims; 

[ApiController]
[Route("api/[controller]")]
public class ContentController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    private int GetCurrentProfileId()
    {
        var profileIdClaim = User.FindFirst("ProfileId")?.Value;
        return int.Parse(profileIdClaim ?? "0");
    }

    [HttpGet("{movieId}")]
    public async Task<ActionResult<MoviePublicDto>> GetMovieDetail(int movieId)
    {
        var movie = await _context.Movies
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieCategories).ThenInclude(mc => mc.Category)
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null) return NotFound();

        var dto = new MoviePublicDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            ReleaseYear = movie.ReleaseYear,
            RuntimeMinutes = movie.RuntimeMinutes,
            ThumbnailUrl = movie.ThumbnailUrl,
            BackdropUrl = movie.BackdropUrl,
            AgeRating = movie.AgeRating,
            AverageRating = movie.AverageRating,
            Genres = movie.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
            Categories = movie.MovieCategories.Select(mc => mc.Category.Name).ToList()
        };

        return Ok(dto);
    }

    [HttpGet("home")]
    public async Task<ActionResult<HomeFeedDto>> GetHomeFeed()
    {
        var categoryRows = await _context.Categories
            .Take(5)
            .Select(c => new HomeFeedRowDto
            {
                RowTitle = c.Name,
                Movies = c.MovieCategories
                    .Take(10)
                    .Select(mc => new MoviePublicDto 
                    {
                        Id = mc.Movie.Id,
                        Title = mc.Movie.Title,
                        ThumbnailUrl = mc.Movie.ThumbnailUrl,
                        AverageRating = mc.Movie.AverageRating
                    }).ToList()
            }).ToListAsync();

        var homeFeed = new HomeFeedDto
        {
            CategoryRows = categoryRows
        };

        return Ok(homeFeed);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<MoviePublicDto>>> SearchMovies([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var movies = await _context.Movies
            .Where(m => EF.Functions.Like(m.Title, $"%{q}%") || EF.Functions.Like(m.Description, $"%{q}%"))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = movies.Select(m => new MoviePublicDto
        {
            Id = m.Id,
            Title = m.Title,
            ThumbnailUrl = m.ThumbnailUrl,
            AverageRating = m.AverageRating
        }).ToList();

        return Ok(dtos);
    }
}