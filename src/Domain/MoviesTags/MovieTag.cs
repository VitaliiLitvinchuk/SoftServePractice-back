using Domain.Movies;
using Domain.Tags;

namespace Domain.MoviesTags;

public class MovieTag(MovieId movieId, TagId tagId)
{
    public MovieId MovieId { get; } = movieId;
    public TagId TagId { get; } = tagId;

    public Movie? Movie { get; }
    public Tag? Tag { get; }

    public static MovieTag New(MovieId movieId, TagId tagId)
        => new(movieId, tagId);
}
