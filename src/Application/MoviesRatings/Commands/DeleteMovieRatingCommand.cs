using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesRatings.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesRatings;
using Domain.Users;
using MediatR;

namespace Application.MoviesRatings.Commands;

public class DeleteMovieRatingCommand : IRequest<Result<MovieRating, MovieRatingException>>
{
    public required Guid MovieId { get; init; }
    public required Guid UserId { get; init; }
}

public class DeleteMovieRatingCommandHandler(IBaseRepository<MovieRating> repository, IBaseQuery<MovieRating> query) : IRequestHandler<DeleteMovieRatingCommand, Result<MovieRating, MovieRatingException>>
{
    public async Task<Result<MovieRating, MovieRatingException>> Handle(DeleteMovieRatingCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var userId = new UserId(request.UserId);
        var entity = MovieRating.New(userId, movieId, 0);

        var result = await query.Get(cancellation, x => x.MovieId == entity.MovieId && x.UserId == entity.UserId);

        return await result.Match(
            async entity => await DeleteEntity(entity, cancellation),
            () => Task.FromResult<Result<MovieRating, MovieRatingException>>(new MovieRatingNotFoundException(entity.MovieId, entity.UserId))
        );
    }

    private async Task<Result<MovieRating, MovieRatingException>> DeleteEntity(MovieRating entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieRatingUnknownException(entity.MovieId, entity.UserId, exception);
        }
    }
}
