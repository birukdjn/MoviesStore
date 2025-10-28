using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MoviesStore.models
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string Director { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;

        public List<Favorite> Favorites { get; set; } = new();
        public List<Rating> Ratings { get; set; } = new();

        public int CategoryId { get; set; }      
        public Category Category { get; set; } = null!;


    }
}
