

namespace MoviesStore.models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<MovieCategory> MovieCategories { get; set; } = [];
    }
}
