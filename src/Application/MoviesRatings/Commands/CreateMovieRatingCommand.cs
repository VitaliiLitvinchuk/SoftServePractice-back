using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesRatings.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesRatings;
using Domain.Users;
using MediatR;

namespace Application.MoviesRatings.Commands;

public class CreateMovieRatingCommand : IRequest<Result<MovieRating, MovieRatingException>>
{
    public required Guid MovieId { get; init; }
    public required Guid UserId { get; init; }
    public required int Rating { get; init; }
}

public class CreateMovieRatingCommandHandler(IBaseRepository<MovieRating> repository, IBaseQuery<MovieRating> query, IBaseQuery<Movie> movieQuery, IBaseQuery<User> userQuery) : IRequestHandler<CreateMovieRatingCommand, Result<MovieRating, MovieRatingException>>
{
    public async Task<Result<MovieRating, MovieRatingException>> Handle(CreateMovieRatingCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var userId = new UserId(request.UserId);
        var result = await query.Get(cancellation, x => x.MovieId == movieId && x.UserId == userId);

        return await result.Match(
            entity => Task.FromResult<Result<MovieRating, MovieRatingException>>(new MovieRatingAlreadyExistsException(entity.MovieId, entity.UserId)),
            async () =>
            {
                var movie = await movieQuery.Get(cancellation, x => x.Id == movieId);

                return await movie.Match(
                    async movie =>
                    {
                        var user = await userQuery.Get(cancellation, x => x.Id == userId);

                        return await user.Match(
                            async user => await CreateEntity(MovieRating.New(userId, movieId, request.Rating), cancellation),
                            () => Task.FromResult<Result<MovieRating, MovieRatingException>>(new UserForMovieRatingNotFoundException(userId))
                        );
                    },
                    () => Task.FromResult<Result<MovieRating, MovieRatingException>>(new MovieForMovieRatingNotFoundException(movieId))
                );
            }
        );
    }

    private async Task<Result<MovieRating, MovieRatingException>> CreateEntity(MovieRating entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieRatingUnknownException(entity.MovieId, entity.UserId, exception);
        }
    }
}
