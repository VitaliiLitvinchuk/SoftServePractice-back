namespace Api.Dtos.Movies;

public record CreateMovieDto(string Name, long Duration, string TrailerUrl, IFormFile Image, string Description, DateTime ReleaseDate);
