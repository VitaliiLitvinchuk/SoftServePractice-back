namespace Api.Dtos.MoviesRating;

public record UpdateMovieRatingDto(Guid MovieId, Guid UserId, int Rating);
