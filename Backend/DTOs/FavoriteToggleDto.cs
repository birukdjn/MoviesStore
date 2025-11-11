using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    // Used for POST /api/Favorites/toggle
    public class FavoriteToggleDto
    {
        [Required]
        // The ID of the movie the current profile wants to add or remove
        public int MovieId { get; set; }
    }
}