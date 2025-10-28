
namespace MoviesStore.models
{
    public class Profile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public List<Favorite> Favorites { get; set; } = new();
        public List<Rating> Ratings { get; set; } = new();
    }
}
