using Domain.Genres;

namespace Application.Genres.Exceptions;

public class GenreException(GenreId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public GenreId Id { get; } = id;
}

public class GenreNotFoundException(GenreId id) : GenreException(id, $"Genre {id} not found.");
public class GenreNameAlreadyExistsException(GenreId id, string name) : GenreException(id, $"Genre with name {name} already exists.");
public class GenreUnknownException(GenreId id, Exception innerException) : GenreException(id, $"Genre {id} is unknown.", innerException);
public class GenreHasReleationsException(GenreId id) : GenreException(id, $"Genre {id} has relations.");