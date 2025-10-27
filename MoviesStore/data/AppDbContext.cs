using Microsoft.EntityFrameworkCore;
using MoviesStore.models;

namespace MoviesStore.data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Profile> Profiles { get; set; }



    }
}
