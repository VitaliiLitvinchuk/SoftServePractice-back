using Domain.Genres;

namespace Api.Dtos.Genres;

public record GenreDto(Guid Id, string Name)
{
    public static GenreDto FromDomainModel(Genre genre)
        => new(genre.Id.Value, genre.Name);

    public static Genre ToDomainModel(GenreDto dto)
        => Genre.New(new(dto.Id), dto.Name);
}
