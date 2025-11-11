using Microsoft.EntityFrameworkCore;
using Backend.models;

namespace Backend.data
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

            User admin;

            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                admin = new User
                {
                    Username = "Birukdjn",
                    Email = "Birukedjn@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Birukdjn@8325"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(admin);
                context.SaveChanges();
            }
            else
            {
                admin = context.Users.First(u => u.Role == "Admin");
            }

            // Seed admin profile if not exists
            if (!context.Profiles.Any(p => p.UserId == admin.Id))
            {
                context.Profiles.Add(new Profile
                {
                    Name = "Biruk",
                    Avatar = "/avatars/biruk.png",
                    IsKidsProfile = false,
                    UserId = admin.Id
                });
                context.SaveChanges();
            }




            // --- 2. Seed Categories ---
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
                context.SaveChanges(); 
            }

            // --- 3. Seed Genres ---
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
                context.SaveChanges(); 
            }

            if (!context.Movies.Any())
            {
                
                var adminUser = context.Users.First(u => u.Role == "Admin");

                var moviesCategory = context.Categories.First(c => c.Name == "Movies");
                var seriesCategory = context.Categories.First(c => c.Name == "Series");
                var documentariesCategory = context.Categories.First(c => c.Name == "Documentaries");
                var specialsCategory = context.Categories.First(c => c.Name == "Specials");

                var actionGenre = context.Genres.First(g => g.Name == "Action");
                var comedyGenre = context.Genres.First(g => g.Name == "Comedies");
                var dramaGenre = context.Genres.First(g => g.Name == "Dramas");
                var sciFiGenre = context.Genres.First(g => g.Name == "Sci-Fi & Fantasy");
                var familyGenre = context.Genres.First(g => g.Name == "Children & Family");
                var crimeGenre = context.Genres.First(g => g.Name == "Crime");
                var horrorGenre = context.Genres.First(g => g.Name == "Horror");
                var independentGenre = context.Genres.First(g => g.Name == "Independent");
                var animeGenre = context.Genres.First(g => g.Name == "Anime");
                var standUpGenre = context.Genres.First(g => g.Name == "Stand-Up Comedy");
                var romanceGenre = context.Genres.First(g => g.Name == "Romance");
                var sportsGenre = context.Genres.First(g => g.Name == "Sports");
                var thrillerGenre = context.Genres.First(g => g.Name == "Thrillers");
                var cultGenre = context.Genres.First(g => g.Name == "Cult");
                var classicGenre = context.Genres.First(g => g.Name == "Classic");


                var basicUser = new User
                {
                    Username = "user",
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                    Role = "User",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(basicUser);
                context.SaveChanges();

                var mainProfile = new Profile
                {
                    Name = "User Profile",
                    Avatar = "/avatars/user.png",
                    IsKidsProfile = false,
                    UserId = basicUser.Id
                };
                context.Profiles.Add(mainProfile);

                var DadProfile = new Profile
                {
                    Name = "User Profile",
                    Avatar = "/avatars/dad.png",
                    IsKidsProfile = false,
                    UserId = basicUser.Id
                };
                context.Profiles.Add(DadProfile);

                var kidsProfile = new Profile
                {
                    Name = "Kids Zone",
                    Avatar = "/avatars/kid.png",
                    IsKidsProfile = true,
                    UserId = basicUser.Id
                };
                context.Profiles.Add(kidsProfile);
                context.SaveChanges(); 

                var movies = new List<Movie>
                {
                    new() {
                        Title = "Cosmic Guardian",
                        ReleaseYear = 2025,
                        Director = "Elena Rios",
                        Description = "A futuristic soldier must protect the galaxy's last relic.",
                        RuntimeMinutes = 145,
                        AgeRating = "PG-13",
                        IsOriginal = true,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = moviesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new() { GenreId = actionGenre.Id },
                            new () { GenreId = sciFiGenre.Id }
                        }
                    },

                    // 2. Family Comedy
                    new() {
                        Title = "Dog's Day Out",
                        ReleaseYear = 2023,
                        Director = "Paul King",
                        Description = "A talking golden retriever gets lost on a hilarious journey home.",
                        RuntimeMinutes = 98,
                        AgeRating = "TV-PG",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = moviesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = comedyGenre.Id },
                            new MovieGenre { GenreId = familyGenre.Id }
                        }
                    },

                    // 3. Historical Drama Series
                    new() {
                        Title = "The Crown of Fire (S1 E1)",
                        ReleaseYear = 2022,
                        Director = "Sofia Bell",
                        Description = "The dramatic start of a new royal dynasty.",
                        RuntimeMinutes = 55,
                        AgeRating = "TV-MA",
                        IsOriginal = true,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = seriesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = dramaGenre.Id },
                            new MovieGenre { GenreId = classicGenre.Id }
                        }
                    },

                    // 4. True Crime Documentary
                    new() {
                        Title = "Shadows in the System",
                        ReleaseYear = 2024,
                        Director = "Dan Evans",
                        Description = "A deep dive into a notorious, unsolved cold case.",
                        RuntimeMinutes = 110,
                        AgeRating = "TV-MA",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = documentariesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = dramaGenre.Id },
                            new MovieGenre { GenreId = crimeGenre.Id }
                        }
                    },

                    // 5. Indie Horror Film
                    new() {
                        Title = "The Whispering Woods",
                        ReleaseYear = 2021,
                        Director = "Liam O'Connell",
                        Description = "Teenagers unleash an ancient evil in a remote forest cabin.",
                        RuntimeMinutes = 88,
                        AgeRating = "R",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = moviesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = horrorGenre.Id },
                            new MovieGenre { GenreId = independentGenre.Id }
                        }
                    },

                    // 6. Animated Anime Series
                    new() {
                        Title = "Cyber Samurai (Pilot)",
                        ReleaseYear = 2024,
                        Director = "Kenji Sato",
                        Description = "In a neon city, a disgraced warrior seeks redemption.",
                        RuntimeMinutes = 24,
                        AgeRating = "TV-14",
                        IsOriginal = true,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = seriesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = actionGenre.Id },
                            new MovieGenre { GenreId = animeGenre.Id }
                        }
                    },

                    // 7. Stand-Up Special
                    new() {
                        Title = "Mark Z. Live in Vegas",
                        ReleaseYear = 2023,
                        Director = "Self",
                        Description = "Comedian Mark Z. delivers an hour of razor-sharp social commentary.",
                        RuntimeMinutes = 60,
                        AgeRating = "TV-MA",
                        IsOriginal = true,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = specialsCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = standUpGenre.Id }
                        }
                    },

                    // 8. Romantic Comedy
                    new() {
                        Title = "Coffee Shop Connection",
                        ReleaseYear = 2020,
                        Director = "Mia T.",
                        Description = "Two strangers meet every morning, leading to an unexpected romance.",
                        RuntimeMinutes = 105,
                        AgeRating = "PG",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = moviesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = comedyGenre.Id },
                            new MovieGenre { GenreId = romanceGenre.Id }
                        }
                    },

                    // 9. Sports Documentary
                    new() {
                        Title = "The Unbroken Record",
                        ReleaseYear = 2019,
                        Director = "Robert L.",
                        Description = "Chronicling the career of a legendary but controversial marathon runner.",
                        RuntimeMinutes = 95,
                        AgeRating = "TV-14",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = documentariesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = sportsGenre.Id }
                        }
                    },

                    // 10. Cult Thriller
                    new() {
                        Title = "Midnight Drive",
                        ReleaseYear = 2005,
                        Director = "J. J. Curtis",
                        Description = "A man picks up a mysterious passenger, leading to a night of terror.",
                        RuntimeMinutes = 92,
                        AgeRating = "R",
                        IsOriginal = false,
                        MovieCategories = new List<MovieCategory> { new MovieCategory { CategoryId = moviesCategory.Id } },
                        MovieGenres = new List<MovieGenre>
                        {
                            new MovieGenre { GenreId = thrillerGenre.Id },
                            new MovieGenre { GenreId = cultGenre.Id }
                        }
                    }
                };

                context.Movies.AddRange(movies);
                context.SaveChanges(); // Save the 10 movies

                // --- D. Seed Relationships (Ratings, Favorites, Playback) ---

                // Rating (Main Profile rates Movie 1)
                context.Ratings.Add(new Rating
                {
                    MovieId = movies[0].Id,
                    ProfileId = mainProfile.Id,
                    Score = 5,
                    RatedDate = DateTime.UtcNow
                });

                // Favorite (Kids Profile favorites Movie 4)
                context.Favorites.Add(new Favorite
                {
                    MovieId = movies[3].Id,
                    ProfileId = kidsProfile.Id,
                    AddedDate = DateTime.UtcNow.AddHours(-1)
                });

                // Playback Position (Main Profile watched Movie 2)
                context.PlaybackPositions.Add(new PlaybackPosition
                {
                    MovieId = movies[1].Id,
                    ProfileId = mainProfile.Id,
                    PositionInSeconds = 600,
                    TotalDurationInSeconds = 1500,
                    LastWatchedDate = DateTime.UtcNow.AddMinutes(-30)
                });

                // Final save for all relationships
                context.SaveChanges();
            }
        }
    }
}