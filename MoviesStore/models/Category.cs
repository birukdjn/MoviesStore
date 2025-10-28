

namespace MoviesStore.models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // One category has many movies
        public List<Movie> Movies { get; set; } = new();
    }
}
