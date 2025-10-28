using Microsoft.AspNetCore.Mvc;
using MoviesStore.Data;
using MoviesStore.models;


namespace MoviesStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            if (_context.Categories.Any(c => c.Name.ToLower() == name.ToLower()))
                return BadRequest("Category already exists.");

            var category = new Category { Name = name };
            _context.Categories.Add(category);
            _context.SaveChanges();

            return Ok(category);
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = _context.Categories.ToList();
            return Ok(categories);
        }
    }
}
