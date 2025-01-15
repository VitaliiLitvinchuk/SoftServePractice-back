using System.Reflection;
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
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GenreTag> GenreTags => Set<GenreTag>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieActor> MoviesActors => Set<MovieActor>();
    public DbSet<MovieGenre> MoviesGenres => Set<MovieGenre>();
    public DbSet<MovieRating> MoviesRatings => Set<MovieRating>();
    public DbSet<MovieTag> MoviesTags => Set<MovieTag>();
    public DbSet<PurchaseHistory> PurchaseHistories => Set<PurchaseHistory>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
