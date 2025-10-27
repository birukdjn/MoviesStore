
namespace MoviesStore.models
{
    public class Rating
    {
        public int Id { get; set; }
        
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int Score { get; set; }
        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;


    }
}
