using System.ComponentModel.DataAnnotations;

namespace MoviesStore.Models.DTOs
{
    public class MovieCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title must not exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(50, ErrorMessage = "Genre must not exceed 50 characters")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Release Year is required")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Director is required")]
        public string Director { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }
    }
}
