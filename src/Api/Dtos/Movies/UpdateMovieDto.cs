namespace Api.Dtos.Movies;

public record UpdateMovieDto(Guid Id, string Name, int Duration, string TrailerUrl, IFormFile? Image, string Description, DateTime ReleaseDate);
