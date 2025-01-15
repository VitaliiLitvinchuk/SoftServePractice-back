using Domain.Actors;
using Domain.Movies;

namespace Domain.MoviesActors;

public class MovieActor(MovieId movieId, ActorId actorId)
{
    public MovieId MovieId { get; } = movieId;
    public ActorId ActorId { get; } = actorId;

    public Movie? Movie { get; }
    public Actor? Actor { get; }

    public static MovieActor New(MovieId movieId, ActorId actorId)
        => new(movieId, actorId);
}
