using Api.Dtos.Movies;
using Api.Dtos.Tags;
using Domain.MoviesTags;

namespace Api.Dtos.MoviesTags;

public record MovieTagDto(Guid MovieId, Guid TagId, MovieDto? Movie, TagDto? Tag)
{
    public static MovieTagDto FromDomainModel(MovieTag movieTag)
        => new(movieTag.MovieId.Value, movieTag.TagId.Value,
            movieTag.Movie is null ? null : MovieDto.FromDomainModel(movieTag.Movie),
            movieTag.Tag is null ? null : TagDto.FromDomainModel(movieTag.Tag));
}
