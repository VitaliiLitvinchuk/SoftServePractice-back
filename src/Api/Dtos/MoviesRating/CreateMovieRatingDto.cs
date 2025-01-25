namespace Api.Dtos.MoviesRating;

public record CreateMovieRatingDto(Guid MovieId, Guid UserId, int Rating);
