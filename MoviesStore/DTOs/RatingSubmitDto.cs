using System.ComponentModel.DataAnnotations;

namespace MoviesStore.DTOs
{
    public class RatingSubmitDto
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [Range(1, 5)] 
        public int Score { get; set; }
    }
}