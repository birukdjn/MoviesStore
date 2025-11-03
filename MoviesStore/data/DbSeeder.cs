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

                if (!context.Profiles.Any(p => p.UserId == admin.Id))
                {
                    var adminProfile = new Profile
                    {
                        FirstName = "Biruk",
                        LastName = "Djn",
                        Email = "birukedjn@gmail.com",
                        PhoneNumber = "123-456-7890",
                        Address = "123 Admin St",
                        City = "AdminCity",
                        Country = "AdminCountry",
                        State = "AdminState",
                        ZipCode = "12345",
                        DateOfBirth = new DateTime(1990, 1, 1),
                        User = admin
                    };
                    context.Profiles.Add(adminProfile);

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

                    }







                }
            }

        }
    }
}
