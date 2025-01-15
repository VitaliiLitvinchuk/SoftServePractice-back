using Domain.Genres;
using Domain.Movies;

namespace Domain.MoviesGenres;

public class MovieGenre(MovieId movieId, GenreId genreId)
{
    public MovieId MovieId { get; } = movieId;
    public GenreId GenreId { get; } = genreId;

    public Movie? Movie { get; }
    public Genre? Genre { get; }

    public static MovieGenre New(MovieId movieId, GenreId genreId)
        => new(movieId, genreId);
}
