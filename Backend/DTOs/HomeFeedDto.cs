namespace Backend.DTOs
{
    public class HomeFeedDto
    {
        public PlaybackDisplayDto ContinueWatchingRow { get; set; } = null!;
        public List<HomeFeedRowDto> CategoryRows { get; set; } = [];
    }
}
