using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesGenres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.Movies;
using Domain.MoviesGenres;
using MediatR;

namespace Application.MoviesGenres.Commands;

public class DeleteMovieGenreCommand : IRequest<Result<MovieGenre, MovieGenreException>>
{
    public required Guid MovieId { get; init; }
    public required Guid GenreId { get; init; }
}

public class DeleteMovieGenreCommandHandler(IBaseRepository<MovieGenre> repository, IBaseQuery<MovieGenre> query) : IRequestHandler<DeleteMovieGenreCommand, Result<MovieGenre, MovieGenreException>>
{
    public async Task<Result<MovieGenre, MovieGenreException>> Handle(DeleteMovieGenreCommand request, CancellationToken cancellationToken)
    {
        var movieId = new MovieId(request.MovieId);
        var genreId = new GenreId(request.GenreId);

        var entity = MovieGenre.New(movieId, genreId);

        var result = await query.Get(cancellationToken, x => x.MovieId == entity.MovieId && x.GenreId == entity.GenreId);

        return await result.Match(
            entity => DeleteEntity(entity, cancellationToken),
            () => Task.FromResult<Result<MovieGenre, MovieGenreException>>(new MovieGenreNotFoundException(entity.MovieId, entity.GenreId))
        );
    }

    private async Task<Result<MovieGenre, MovieGenreException>> DeleteEntity(MovieGenre entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieGenreUnknownException(entity.MovieId, entity.GenreId, exception);
        }
    }
}
