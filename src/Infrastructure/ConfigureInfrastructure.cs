using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
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
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        var entityTypes = new[]
        {
            typeof(Actor), typeof(Genre), typeof(GenreTag), typeof(Hall), typeof(Movie),
            typeof(MovieActor), typeof(MovieGenre), typeof(MovieRating), typeof(MovieTag),
            typeof(PurchaseHistory), typeof(Role), typeof(Seat), typeof(Session),
            typeof(Status), typeof(Tag), typeof(Ticket), typeof(User)
        };

        foreach (var entityType in entityTypes)
        {
            var repositoryType = typeof(BaseRepository<>).MakeGenericType(entityType);
            var baseQueryType = typeof(IBaseQuery<>).MakeGenericType(entityType);
            var baseRepositoryType = typeof(IBaseRepository<>).MakeGenericType(entityType);

            services.AddScoped(repositoryType);
            services.AddScoped(baseQueryType, provider => provider.GetRequiredService(repositoryType));
            services.AddScoped(baseRepositoryType, provider => provider.GetRequiredService(repositoryType));
        }
    }
}
