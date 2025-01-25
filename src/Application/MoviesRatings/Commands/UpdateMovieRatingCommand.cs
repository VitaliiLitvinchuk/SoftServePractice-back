using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesRatings.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesRatings;
using Domain.Users;
using MediatR;

namespace Application.MoviesRatings.Commands;

public class UpdateMovieRatingCommand : IRequest<Result<MovieRating, MovieRatingException>>
{
    public required Guid MovieId { get; init; }
    public required Guid UserId { get; init; }
    public required int Rating { get; init; }
}

public class UpdateMovieRatingCommandHandler(IBaseRepository<MovieRating> repository, IBaseQuery<MovieRating> query) : IRequestHandler<UpdateMovieRatingCommand, Result<MovieRating, MovieRatingException>>
{
    public async Task<Result<MovieRating, MovieRatingException>> Handle(UpdateMovieRatingCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var userId = new UserId(request.UserId);
        var result = await query.Get(cancellation, x => x.MovieId == movieId && x.UserId == userId);

        return await result.Match(
            async entity => await UpdateEntity(entity, request.Rating, cancellation),
            () => Task.FromResult<Result<MovieRating, MovieRatingException>>(new MovieRatingNotFoundException(movieId, userId))
        );
    }

    private async Task<Result<MovieRating, MovieRatingException>> UpdateEntity(MovieRating entity, int rating, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(rating);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieRatingUnknownException(entity.MovieId, entity.UserId, exception);
        }
    }
}
