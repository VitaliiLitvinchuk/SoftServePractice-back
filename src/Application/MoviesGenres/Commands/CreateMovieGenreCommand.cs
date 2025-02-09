using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesGenres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.Movies;
using Domain.MoviesGenres;
using MediatR;

namespace Application.MoviesGenres.Commands;

public class CreateMovieGenreCommand : IRequest<Result<MovieGenre, MovieGenreException>>
{
    public required Guid MovieId { get; init; }
    public required Guid GenreId { get; init; }
}

public class CreateMovieGenreCommandHandler(IBaseRepository<MovieGenre> repository, IBaseQuery<MovieGenre> query, IBaseQuery<Movie> movieQuery, IBaseQuery<Genre> genreQuery) : IRequestHandler<CreateMovieGenreCommand, Result<MovieGenre, MovieGenreException>>
{
    public async Task<Result<MovieGenre, MovieGenreException>> Handle(CreateMovieGenreCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var genreId = new GenreId(request.GenreId);

        var entity = MovieGenre.New(movieId, genreId);

        var result = await query.Get(cancellation, x => x.MovieId == entity.MovieId && x.GenreId == entity.GenreId);

        return await result.Match(
            entity => Task.FromResult<Result<MovieGenre, MovieGenreException>>(new MovieGenreAlreadyExistsException(entity.MovieId, entity.GenreId)),
            async () =>
            {
                var movie = await movieQuery.Get(cancellation, x => x.Id == entity.MovieId);

                return await movie.Match(
                    async movie =>
                    {
                        var genre = await genreQuery.Get(cancellation, x => x.Id == entity.GenreId);

                        return await genre.Match(
                            async genre => await CreateEntity(entity, cancellation),
                            () => Task.FromResult<Result<MovieGenre, MovieGenreException>>(new GenreForMovieGenreNotFoundException(entity.GenreId))
                        );
                    },
                    () => Task.FromResult<Result<MovieGenre, MovieGenreException>>(new MovieForMovieGenreNotFoundException(entity.MovieId))
                );
            }
        );
    }

    private async Task<Result<MovieGenre, MovieGenreException>> CreateEntity(MovieGenre entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieGenreUnknownException(entity.MovieId, entity.GenreId, exception);
        }
    }
}
