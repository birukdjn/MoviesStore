using System.ComponentModel.DataAnnotations;

namespace MoviesStore.DTOs
{
    public class PlaybackUpdateDto
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        public int PositionInSeconds { get; set; }
        public int TotalDurationInSeconds { get; set; }
    }
}