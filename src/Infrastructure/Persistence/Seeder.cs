using Application;
using Application.Common.Interfaces.Services;
using Domain.Actors;
using Domain.Genres;
using Domain.GenresTags;
using Domain.Halls;
using Domain.Movies;
using Domain.MoviesActors;
using Domain.MoviesGenres;
using Domain.MoviesRatings;
using Domain.MoviesTags;
using Domain.PurchaseHistories;
using Domain.Roles;
using Domain.Seats;
using Domain.Sessions;
using Domain.Statuses;
using Domain.Tags;
using Domain.Tickets;
using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Infrastructure.Persistence;

public static class Seeder
{
    private static async Task<IEnumerable<T>> HttpRequester<T>(string url, Func<dynamic, T> func)
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var actorResponse = JsonConvert.DeserializeObject<List<dynamic>>(responseBody);

        return actorResponse!.Select(func);
    }

    public static async Task SeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        await SeedRoles(context.Roles);
        await SeedStatuses(context.Statuses);

        await context.SaveChangesAsync();

        if (env.IsDevelopment())
        {
            await SeedActors(context.Actors);
            await context.SaveChangesAsync();

            await SeedTags(context.Tags);
            await context.SaveChangesAsync();

            await SeedGenres(context.Genres);
            await context.SaveChangesAsync();

            await SeedGenreTags(context.GenreTags, context.Genres, context.Tags);
            await context.SaveChangesAsync();

            await SeedHalls(context.Halls);
            await context.SaveChangesAsync();

            // await SeedMovies(context.Movies);
            // await SeedMovieActors(context.MoviesActors);
            // await SeedMovieGenres(context.MoviesGenres);
            // await SeedMovieRatings(context.MoviesRatings);
            // await SeedMovieTags(context.MoviesTags);
            // await SeedPurchaseHistories(context.PurchaseHistories);
            // await SeedSessions(context.Sessions);
            // await SeedTickets(context.Tickets);

            await SeedSeats(context.Seats, context.Halls);
            await context.SaveChangesAsync();

            var hashService = scope.ServiceProvider.GetRequiredService<IHashService>();

            await SeedUsers(context.Users, context.Roles, hashService);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedRoles(DbSet<Role> roles)
    {
        if (!roles.Any())
        {
            IEnumerable<Role> roleList = [
                Role.New(RoleId.New(), Defaults.AdminRole),
                Role.New(RoleId.New(), Defaults.UserRole)
            ];

            await roles.AddRangeAsync(roleList);
        }
    }

    public static async Task SeedStatuses(DbSet<Status> statuses)
    {
        if (!statuses.Any())
        {
            IEnumerable<Status> statusList = [
                Status.New(StatusId.New(), "Pending"),
                Status.New(StatusId.New(), "Active"),
                Status.New(StatusId.New(), "Inactive")
            ];

            await statuses.AddRangeAsync(statusList);
        }
    }

    public static async Task SeedActors(DbSet<Actor> actors)
    {
        if (!actors.Any())
        {
            var actorList = (await HttpRequester("https://api.tvmaze.com/people?page=1", x =>
            {
                try
                {
                    string[] names = $"{x.name}".Split(' ');
                    string imageUrl = "Not Found";

                    if (x.image is not null)
                    {
                        imageUrl = x.image.original ?? "Not Found";
                    }

                    return Actor.New(ActorId.New(), names.First(), names.Last(), x.id, imageUrl) as Actor;
                }
                catch
                {
                    return null;
                }
            })).Where(x => x is not null);

            await actors.AddRangeAsync(actorList!);
        }
    }

    public static async Task SeedTags(DbSet<Tag> tags)
    {
        if (!tags.Any())
        {
            var tagList = await HttpRequester("https://random-word-api.herokuapp.com/word?number=10000", x => Tag.New(TagId.New(), (x as string)!));

            await tags.AddRangeAsync(tagList);
        }
    }

    public static async Task SeedGenres(DbSet<Genre> genres)
    {
        if (!genres.Any())
        {
            var genreList = await HttpRequester("https://random-word-api.herokuapp.com/word?number=300", x => Genre.New(GenreId.New(), (x as string)!));

            await genres.AddRangeAsync(genreList);
        }
    }

    public static async Task SeedGenreTags(DbSet<GenreTag> genreTags, DbSet<Genre> genres, DbSet<Tag> tags)
    {
        if (!genreTags.Any())
        {
            var random = new Random();
            var genreTagList = new List<GenreTag>();

            var genreList = await genres.ToListAsync();
            var tagList = await tags.ToListAsync();

            foreach (var genre in genreList)
            {
                var tagCount = random.Next(1, 10);
                var randomTags = tagList.OrderBy(x => Guid.NewGuid()).Take(tagCount);

                foreach (var tag in randomTags)
                {
                    genreTagList.Add(GenreTag.New(genre.Id, tag.Id));
                }
            }

            await genreTags.AddRangeAsync(genreTagList);
        }
    }


    public static async Task SeedHalls(DbSet<Hall> halls)
    {
        if (!halls.Any())
        {
            var random = new Random();
            var hallList = await HttpRequester("https://random-word-api.herokuapp.com/word?number=10", x => Hall.New(HallId.New(), (x as string)!, (short)random.Next(1, 100)));

            await halls.AddRangeAsync(hallList);
        }
    }

    public static async Task SeedMovies(DbSet<Movie> movies)
    {
        if (!movies.Any())
        {
            // var random = new Random();
            // var movieList = new List<Movie>();

            // for (int i = 1; i <= 200; i++)
            // {
            //     movieList.Add(Movie.New(
            //         MovieId.New(),
            //         $"Movie {i}",
            //         random.Next(3600, 10800),
            //         $"https://www.youtube.com/watch?v=trailer{i}",
            //         $"https://example.com/image{i}.jpg",
            //         $"Description for Movie {i}",
            //         DateTime.Now.AddDays(-random.Next(0, 3650))
            //     ));
            // }

            // await movies.AddRangeAsync(movieList);
        }
    }

    public static async Task SeedMovieActors(DbSet<MovieActor> movieActors)
    {
        if (!movieActors.Any())
        {
            // await movieActors.AddRangeAsync(movieActorList);
        }
    }

    public static async Task SeedMovieGenres(DbSet<MovieGenre> movieGenres)
    {
        if (!movieGenres.Any())
        {
            // await movieGenres.AddRangeAsync(movieGenreList);
        }
    }

    public static async Task SeedMovieRatings(DbSet<MovieRating> movieRatings)
    {
        if (!movieRatings.Any())
        {
            // await movieRatings.AddRangeAsync(movieRatingList);
        }
    }

    public static async Task SeedMovieTags(DbSet<MovieTag> movieTags)
    {
        if (!movieTags.Any())
        {
            // await movieTags.AddRangeAsync(movieTagList);
        }
    }

    public static async Task SeedPurchaseHistories(DbSet<PurchaseHistory> purchaseHistories)
    {
        if (!purchaseHistories.Any())
        {
            // await purchaseHistories.AddRangeAsync(purchaseHistoryList);
        }
    }

    public static async Task SeedSeats(DbSet<Seat> seats, DbSet<Hall> halls)
    {
        if (seats.Count() < 30)
        {
            var random = new Random();
            var seatList = new List<Seat>();

            foreach (var hall in halls)
            {
                var rows = random.Next(5, 15);
                var columns = random.Next(5, 15);

                for (int i = 1; i <= rows; i++)
                {
                    for (int j = 1; j <= columns; j++)
                    {
                        seatList.Add(Seat.New(SeatId.New(), i, j, hall.Id));
                    }
                }
            }

            await seats.AddRangeAsync(seatList);
        }
    }


    public static async Task SeedSessions(DbSet<Session> sessions)
    {
        if (!sessions.Any())
        {
            // await sessions.AddRangeAsync(sessionList);
        }
    }

    public static async Task SeedTickets(DbSet<Ticket> tickets)
    {
        if (!tickets.Any())
        {
            // await tickets.AddRangeAsync(ticketList);
        }
    }

    public static async Task SeedUsers(DbSet<User> users, DbSet<Role> roles, IHashService hashService)
    {
        if (!users.Any())
        {
            var adminRole = roles.Single(x => x.Name == Defaults.AdminRole);
            var userRole = roles.Single(x => x.Name == Defaults.UserRole);

            IEnumerable<User> userList = [
                User.New(UserId.New(), "admin@a.a", hashService.HashPassword("password"), adminRole.Id),
                User.New(UserId.New(), "user@a.a", hashService.HashPassword("password"), userRole.Id)
            ];

            await users.AddRangeAsync(userList);
        }
    }
}