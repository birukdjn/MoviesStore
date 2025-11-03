namespace MoviesStore.DTOs
{
    public class ProfileSelectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public bool IsKidsProfile { get; set; }

        public DateTime? LastActiveDate { get; set; } 
    }
}