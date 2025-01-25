using Domain.MoviesRatings;

namespace Api.Dtos.MoviesRating;

public record MovieRatingDto(Guid MovieId, Guid UserId, int Rate)
{
    public static MovieRatingDto FromDomainModel(MovieRating movieRating)
        => new(movieRating.MovieId.Value, movieRating.UserId.Value, movieRating.Rate);
}
