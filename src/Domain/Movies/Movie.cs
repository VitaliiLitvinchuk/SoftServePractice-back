using Domain.MoviesActors;
using Domain.MoviesGenres;
using Domain.MoviesRatings;
using Domain.MoviesTags;
using Domain.PurchaseHistories;
using Domain.Sessions;

namespace Domain.Movies;

public class Movie(MovieId id, string name, long duration, string trailerUrl, string imageUrl, string description, DateTime releaseDate)
{
    public MovieId Id { get; } = id;
    public string Name { get; private set; } = name;
    public long Duration { get; private set; } = duration;
    public string TrailerUrl { get; private set; } = trailerUrl;
    public string ImageUrl { get; private set; } = imageUrl;
    public string Description { get; private set; } = description;
    public DateTime ReleaseDate { get; private set; } = releaseDate;

    public ICollection<MovieGenre> Genres { get; } = [];
    public ICollection<MovieTag> Tags { get; } = [];
    public ICollection<MovieActor> Actors { get; } = [];
    public ICollection<Session> Sessions { get; } = [];
    public ICollection<MovieRating> Ratings { get; } = [];
    public ICollection<PurchaseHistory> PurchaseHistories { get; } = [];

    public void UpdateDatails(string name, long duration, string trailerUrl, string imageUrl, string description, DateTime releaseDate)
    {
        Name = name;
        Duration = duration;
        TrailerUrl = trailerUrl;
        ImageUrl = imageUrl;
        Description = description;
        ReleaseDate = releaseDate;
    }

    public static Movie New(MovieId id, string name, long duration, string trailerUrl, string imageUrl, string description, DateTime releaseDate)
        => new(id, name, duration, trailerUrl, imageUrl, description, releaseDate);
}
