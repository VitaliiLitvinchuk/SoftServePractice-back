using Domain.Genres;
using Domain.Tags;

namespace Domain.GenresTags;

public class GenreTag(GenreId genreId, TagId tagId)
{
    public GenreId GenreId { get; } = genreId;
    public TagId TagId { get; } = tagId;

    public Genre? Genre { get; }
    public Tag? Tag { get; }

    public static GenreTag New(GenreId genreId, TagId tagId)
        => new(genreId, tagId);
}
