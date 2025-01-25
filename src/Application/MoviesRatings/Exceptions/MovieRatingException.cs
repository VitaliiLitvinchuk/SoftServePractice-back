using Domain.Movies;
using Domain.Users;

namespace Application.MoviesRatings.Exceptions;

public class MovieRatingException(MovieId? movieId, UserId? userId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public MovieId? MovieId { get; } = movieId;
    public UserId? UserId { get; } = userId;
}

public class MovieRatingNotFoundException(MovieId movieId, UserId userId) : MovieRatingException(movieId, userId, $"Movie rating {movieId} for user {userId} not found.");
public class MovieRatingAlreadyExistsException(MovieId movieId, UserId userId) : MovieRatingException(movieId, userId, $"Movie rating {movieId} for user {userId} already exists");
public class UserForMovieRatingNotFoundException(UserId userId) : MovieRatingException(null, userId, $"User {userId} for movie rating not found.");
public class MovieForMovieRatingNotFoundException(MovieId movieId) : MovieRatingException(movieId, null, $"Movie {movieId} for movie rating not found.");
public class MovieRatingUnknownException(MovieId movieId, UserId userId, Exception innerException) : MovieRatingException(movieId, userId, $"Movie rating {movieId} for user {userId} is unknown.", innerException);
