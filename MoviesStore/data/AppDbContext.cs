using Microsoft.EntityFrameworkCore;
using MoviesStore.models;

namespace MoviesStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1️⃣ Profile - Favorite (one-to-many)
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Profile)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2️⃣ Profile - Rating (one-to-many)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Profile)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
