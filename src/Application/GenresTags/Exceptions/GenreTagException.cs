using Domain.Genres;
using Domain.Tags;

namespace Application.GenresTags.Exceptions;

public class GenreTagException(GenreId? genreId, TagId? tagId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public GenreId? GenreId { get; } = genreId;

    public TagId? TagId { get; } = tagId;
}

public class GenreTagNotFoundException(GenreId genreId, TagId tagId) : GenreTagException(genreId, tagId, $"Genre {genreId} or tag {tagId} not found");
public class GenreTagAlreadyExistsException(GenreId genreId, TagId tagId) : GenreTagException(genreId, tagId, $"Relation between genre {genreId} and tag {tagId} already exists");
public class GenreForGenreTagNotFoundException(GenreId genreId) : GenreTagException(genreId, null, $"Genre {genreId} not found");
public class TagForGenreTagNotFoundException(TagId tagId) : GenreTagException(null, tagId, $"Tag {tagId} not found");
public class GenreTagUnknownException(GenreId genreId, TagId tagId, Exception innerException) : GenreTagException(genreId, tagId, $"Relation between genre {genreId} and tag {tagId} is unknown.", innerException);