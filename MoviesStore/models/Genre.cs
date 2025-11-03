namespace MoviesStore.models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<MovieGenre> MovieGenres { get; set; } = [];

    }
}
