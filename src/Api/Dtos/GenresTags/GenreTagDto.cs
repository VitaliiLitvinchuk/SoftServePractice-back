using Api.Dtos.Genres;
using Api.Dtos.Tags;
using Domain.GenresTags;

namespace Api.Dtos.GenresTags;

public record GenreTagDto(Guid GenreId, Guid TagId, GenreDto? Genre, TagDto? Tag)
{
    public static GenreTagDto FromDomainModel(GenreTag genreTag)
        => new(genreTag.GenreId.Value, genreTag.TagId.Value,
            genreTag.Genre is null ? null : GenreDto.FromDomainModel(genreTag.Genre),
            genreTag.Tag is null ? null : TagDto.FromDomainModel(genreTag.Tag));
}
