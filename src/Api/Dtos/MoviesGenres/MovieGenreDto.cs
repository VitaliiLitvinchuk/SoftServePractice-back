using Api.Dtos.Genres;
using Api.Dtos.Movies;
using Domain.MoviesGenres;

namespace Api.Dtos.MoviesGenres;

public record MovieGenreDto(Guid MovieId, Guid GenreId, MovieDto? Movie, GenreDto? Genre)
{
    public static MovieGenreDto FromDomainModel(MovieGenre movieGenre)
        => new(movieGenre.MovieId.Value, movieGenre.GenreId.Value,
            movieGenre.Movie is null ? null : MovieDto.FromDomainModel(movieGenre.Movie),
            movieGenre.Genre is null ? null : GenreDto.FromDomainModel(movieGenre.Genre));
}
