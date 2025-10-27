namespace MoviesStore.models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public List<Favorite> Favorites { get; set; } = new();
        public List<Rating> Ratings { get; set; } = new();

    }
}