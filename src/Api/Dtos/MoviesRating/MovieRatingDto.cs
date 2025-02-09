using Api.Dtos.Movies;
using Api.Dtos.Users;
using Domain.MoviesRatings;

namespace Api.Dtos.MoviesRating;

public record MovieRatingDto(Guid MovieId, Guid UserId, int Rate, MovieDto? Movie, UserDto? User)
{
    public static MovieRatingDto FromDomainModel(MovieRating movieRating)
        => new(movieRating.MovieId.Value, movieRating.UserId.Value, movieRating.Rate,
            movieRating.Movie is null ? null : MovieDto.FromDomainModel(movieRating.Movie),
            movieRating.User is null ? null : UserDto.FromDomainModel(movieRating.User));
}
