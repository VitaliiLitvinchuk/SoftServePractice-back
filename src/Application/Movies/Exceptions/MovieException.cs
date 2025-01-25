using Domain.Movies;

namespace Application.Movies.Exceptions;

public class MovieException(MovieId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public MovieId Id { get; } = id;
}

public class MovieNotFoundException(MovieId id) : MovieException(id, $"Movie {id} not found.");
public class MovieUnknownException(MovieId id, Exception innerException) : MovieException(id, $"Movie {id} is unknown.", innerException);
public class MovieHasReleationsException(MovieId id) : MovieException(id, $"Movie {id} has relations.");
