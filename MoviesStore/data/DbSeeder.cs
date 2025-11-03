using Microsoft.EntityFrameworkCore;
using MoviesStore.Data;
using MoviesStore.models;

namespace MoviesStore.data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch
            {

            }
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Username = "Birukdjn",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Birukdjn@8325"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(admin);
                context.SaveChanges();



                if (!context.Categories.Any())
                {
                    var categories = new List<Category>
                {
                    new() { Name = "Movies"},
                    new() { Name = "Series"},
                    new() { Name = "Documentaries"},
                    new() { Name = "Shorts"},
                    new() { Name = "Specials"},
                    new() { Name = "Live TV"},
                    new() { Name = "News"},
                    new() { Name = "Kids & Family"}

                };
                    context.Categories.AddRange(categories);
                }

                if (!context.Genres.Any())
                {
                    var genres = new List<Genre>
                {
                    new () { Name = "Action" },
                    new () { Name = "Anime" },
                    new () { Name = "Children & Family" },
                    new () { Name = "Classic" },
                    new () { Name = "Comedies" },
                    new () { Name = "Crime" },
                    new () { Name = "Cult" },
                    new () { Name = "Documentaries" },
                    new () { Name = "Dramas" },
                    new () { Name = "Faith & Spirituality" },
                    new () { Name = "Horror" },
                    new () { Name = "Independent" },
                    new () { Name ="International" },
                    new () { Name = "Music" },
                    new () { Name = "Musicals" },
                    new () { Name = "Reality TV" },
                    new () { Name = "Romance" },
                    new () { Name = "Sci-Fi & Fantasy" },
                    new () { Name = "Sports" },
                    new () { Name = "Stand-Up Comedy" },
                    new () { Name = "Thrillers" },
                    new () { Name = "TV Shows" }
                };

                    context.Genres.AddRange(genres);
                }
                if (!context.Movies.Any())
                {
                    // Get some genres
                    var actionGenre = context.Genres.FirstOrDefault(g => g.Name == "Action");
                    var comedyGenre = context.Genres.FirstOrDefault(g => g.Name == "Comedies");
                    var dramaGenre = context.Genres.FirstOrDefault(g => g.Name == "Dramas");
                    var sciFiGenre = context.Genres.FirstOrDefault(g => g.Name == "Sci-Fi & Fantasy");

                    // Get some categories
                    var moviesCategory = context.Categories.FirstOrDefault(c => c.Name == "Movies");
                    var seriesCategory = context.Categories.FirstOrDefault(c => c.Name == "Series");
                    var documentariesCategory = context.Categories.FirstOrDefault(c => c.Name == "Documentaries");

                    // Seed movies
                    var movies = new List<Movie>
                {

                new Movie
                {
                    Title = "Epic Adventure",
                    ReleaseYear = 2023,
                    Director = "John Smith",
                    Description = "An action-packed journey of heroes.",
                    MovieGenres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = actionGenre!.Id }
                    },
                    MovieCategories = new List<MovieCategory>
                    {
                        new MovieCategory { CategoryId = moviesCategory!.Id }
                    }
                },
                new Movie
                {
                    Title = "Laugh Out Loud",
                    ReleaseYear = 2022,
                    Director = "Jane Doe",
                    Description = "A hilarious comedy for the whole family.",
                    MovieGenres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = comedyGenre!.Id }
                    },
                    MovieCategories = new List<MovieCategory>
                    {
                        new MovieCategory { CategoryId = moviesCategory!.Id }
                    }
                },
                new Movie
                {
                    Title = "Space Odyssey",
                    ReleaseYear = 2021,
                    Director = "Alice Johnson",
                    Description = "A thrilling sci-fi adventure through the cosmos.",
                    MovieGenres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = sciFiGenre!.Id }
                    },
                    MovieCategories = new List<MovieCategory>
                    {
                        new MovieCategory { CategoryId = seriesCategory!.Id }
                    }
                },
                new Movie
                {
                    Title = "Life of Drama",
                    ReleaseYear = 2020,
                    Director = "Michael Lee",
                    Description = "An emotional drama that touches the heart.",
                    MovieGenres = new List<MovieGenre>
                    {
                        new MovieGenre { GenreId = dramaGenre!.Id }
                    },
                    MovieCategories = new List<MovieCategory>
                    {
                        new MovieCategory { CategoryId = documentariesCategory!.Id }
                    }
                }
            };

                    context.Movies.AddRange(movies);
                    context.SaveChanges();
                }
            }
        }
    }
}
        