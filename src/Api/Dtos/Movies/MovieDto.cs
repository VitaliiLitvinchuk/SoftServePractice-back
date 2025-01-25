using Domain.Movies;

namespace Api.Dtos.Movies;

public record MovieDto(Guid Id, string Name, long Duration, string TrailerUrl, string ImageUrl, string Description, DateTime ReleaseDate)
{
    public static MovieDto FromDomainModel(Movie movie)
        => new(movie.Id.Value, movie.Name, movie.Duration, movie.TrailerUrl, movie.ImageUrl, movie.Description, movie.ReleaseDate);
}
