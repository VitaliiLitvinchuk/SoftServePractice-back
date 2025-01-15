using Domain.MoviesActors;
using Domain.MoviesGenres;
using Domain.MoviesRatings;
using Domain.MoviesTags;
using Domain.PurchaseHistories;
using Domain.Sessions;

namespace Domain.Movies;

public class Movie(MovieId movieId, string name, long duration)
{
    public MovieId Id { get; } = movieId;
    public string Name { get; private set; } = name;
    public long Duration { get; private set; } = duration;

    public ICollection<MovieGenre> Genres { get; } = [];
    public ICollection<MovieTag> Tags { get; } = [];
    public ICollection<MovieActor> Actors { get; } = [];
    public ICollection<Session> Sessions { get; } = [];
    public ICollection<MovieRating> Ratings { get; } = [];
    public ICollection<PurchaseHistory> PurchaseHistories { get; } = [];

    public void UpdateDatails(string name, long duration)
    {
        Name = name;
        Duration = duration;
    }

    public static Movie New(MovieId movieId, string name, long duration)
        => new(movieId, name, duration);
}
