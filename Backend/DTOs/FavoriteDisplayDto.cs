
namespace Backend.DTOs
{
    public class FavoriteDisplayDto
    {
        public MoviePublicDto Movie { get; set; } = new();
        public DateTime AddedDate { get; set; }
    }
}