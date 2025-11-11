
namespace Backend.DTOs
{
    public class HomeFeedRowDto
    {
        public string RowTitle { get; set; } = string.Empty;
        public List<MoviePublicDto> Movies { get; set; } = [];
    }
}
