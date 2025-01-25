using Domain.Movies;
using Domain.Tags;

namespace Application.MoviesTags.Exceptions;

public class MovieTagException(MovieId? movieId, TagId? tagId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public MovieId? MovieId { get; } = movieId;
    public TagId? TagId { get; } = tagId;
}

public class MovieTagNotFoundException(MovieId movieId, TagId tagId) : MovieTagException(movieId, tagId, $"Movie {movieId} or tag {tagId} not found.");
public class MovieTagAlreadyExistsException(MovieId movieId, TagId tagId) : MovieTagException(movieId, tagId, $"Relation between movie {movieId} and tag {tagId} already exists");
public class MovieForMovieTagNotFoundException(MovieId movieId) : MovieTagException(movieId, null, $"Movie {movieId} for movie tag not found.");
public class TagForMovieTagNotFoundException(TagId tagId) : MovieTagException(null, tagId, $"Tag {tagId} for movie tag not found.");
public class MovieTagUnknownException(MovieId movieId, TagId tagId, Exception innerException) : MovieTagException(movieId, tagId, $"Relation between movie {movieId} and tag {tagId} is unknown.", innerException);
