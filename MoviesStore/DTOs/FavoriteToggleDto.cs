using System.ComponentModel.DataAnnotations;

namespace MoviesStore.DTOs
{
    // Used for POST /api/Favorites/toggle
    public class FavoriteToggleDto
    {
        [Required]
        // The ID of the movie the current profile wants to add or remove
        public int MovieId { get; set; }
    }
}