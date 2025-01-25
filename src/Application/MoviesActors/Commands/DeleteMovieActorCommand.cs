using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesActors.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Actors;
using Domain.Movies;
using Domain.MoviesActors;
using MediatR;

namespace Application.MoviesActors.Commands;

public class DeleteMovieActorCommand : IRequest<Result<MovieActor, MovieActorException>>
{
    public required Guid MovieId { get; init; }
    public required Guid ActorId { get; init; }
}

public class DeleteMovieActorCommandHandler(IBaseRepository<MovieActor> repository, IBaseQuery<MovieActor> query) : IRequestHandler<DeleteMovieActorCommand, Result<MovieActor, MovieActorException>>
{
    public async Task<Result<MovieActor, MovieActorException>> Handle(DeleteMovieActorCommand request, CancellationToken cancellationToken)
    {
        var movieId = new MovieId(request.MovieId);
        var actorId = new ActorId(request.ActorId);

        var entity = MovieActor.New(movieId, actorId);

        var result = await query.Get(cancellationToken, x => x.MovieId == entity.MovieId && x.ActorId == entity.ActorId);

        return await result.Match(
            entity => DeleteEntity(entity, cancellationToken),
            () => Task.FromResult<Result<MovieActor, MovieActorException>>(new MovieActorNotFoundException(entity.MovieId, entity.ActorId))
        );
    }

    private async Task<Result<MovieActor, MovieActorException>> DeleteEntity(MovieActor entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieActorUnknownException(entity.MovieId, entity.ActorId, exception);
        }
    }
}
