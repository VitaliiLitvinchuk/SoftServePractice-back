using Domain.Actors;
using Domain.Movies;

namespace Application.MoviesActors.Exceptions;

public class MovieActorException(MovieId? movieId, ActorId? actorId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public MovieId? MovieId { get; } = movieId;
    public ActorId? ActorId { get; } = actorId;
}

public class MovieActorNotFoundException(MovieId movieId, ActorId actorId) : MovieActorException(movieId, actorId, $"Movie {movieId} or actor {actorId} not found.");
public class MovieActorAlreadyExistsException(MovieId movieId, ActorId actorId) : MovieActorException(movieId, actorId, $"Relation between movie {movieId} and actor {actorId} already exists");
public class MovieForMovieActorNotFoundException(MovieId movieId) : MovieActorException(movieId, null, $"Movie {movieId} for movie actor not found.");
public class ActorForMovieActorNotFoundException(ActorId actorId) : MovieActorException(null, actorId, $"Actor {actorId} for movie actor not found.");
public class MovieActorUnknownException(MovieId movieId, ActorId actorId, Exception innerException) : MovieActorException(movieId, actorId, $"Relation between movie {movieId} and actor {actorId} is unknown.", innerException);