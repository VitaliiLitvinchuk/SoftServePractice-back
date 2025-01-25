using Domain.Genres;
using Domain.Movies;

namespace Application.MoviesGenres.Exceptions;

public class MovieGenreException(MovieId? movieId, GenreId? genreId, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public MovieId? MovieId { get; } = movieId;
    public GenreId? GenreId { get; } = genreId;
}

public class MovieGenreNotFoundException(MovieId movieId, GenreId genreId) : MovieGenreException(movieId, genreId, $"Movie {movieId} or genre {genreId} not found.");
public class MovieGenreAlreadyExistsException(MovieId movieId, GenreId genreId) : MovieGenreException(movieId, genreId, $"Relation between movie {movieId} and genre {genreId} already exists");
public class MovieForMovieGenreNotFoundException(MovieId movieId) : MovieGenreException(movieId, null, $"Movie {movieId} for movie genre not found.");
public class GenreForMovieGenreNotFoundException(GenreId genreId) : MovieGenreException(null, genreId, $"Genre {genreId} for movie genre not found.");
public class MovieGenreUnknownException(MovieId movieId, GenreId genreId, Exception innerException) : MovieGenreException(movieId, genreId, $"Relation between movie {movieId} and genre {genreId} is unknown.", innerException);