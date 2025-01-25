using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesActors.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Actors;
using Domain.Movies;
using Domain.MoviesActors;
using MediatR;

namespace Application.MoviesActors.Commands;

public class CreateMovieActorCommand : IRequest<Result<MovieActor, MovieActorException>>
{
    public required Guid MovieId { get; init; }
    public required Guid ActorId { get; init; }
}

public class CreateMovieActorCommandHandler(IBaseRepository<MovieActor> repository, IBaseQuery<MovieActor> query, IBaseQuery<Movie> movieQuery, IBaseQuery<Actor> actorQuery) : IRequestHandler<CreateMovieActorCommand, Result<MovieActor, MovieActorException>>
{
    public async Task<Result<MovieActor, MovieActorException>> Handle(CreateMovieActorCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var actorId = new ActorId(request.ActorId);

        var entity = MovieActor.New(movieId, actorId);

        var result = await query.Get(cancellation, x => x.MovieId == entity.MovieId && x.ActorId == entity.ActorId);

        return await result.Match(
            entity => Task.FromResult<Result<MovieActor, MovieActorException>>(new MovieActorAlreadyExistsException(entity.MovieId, entity.ActorId)),
            async () =>
            {
                var movie = await movieQuery.Get(cancellation, x => x.Id == entity.MovieId);

                return await movie.Match(
                    async movie =>
                    {
                        var actor = await actorQuery.Get(cancellation, x => x.Id == entity.ActorId);

                        return await actor.Match(
                            async actor => await CreateEntity(entity, cancellation),
                            () => Task.FromResult<Result<MovieActor, MovieActorException>>(new ActorForMovieActorNotFoundException(entity.ActorId))
                        );
                    },
                    () => Task.FromResult<Result<MovieActor, MovieActorException>>(new MovieForMovieActorNotFoundException(entity.MovieId))
                );
            }
        );
    }

    private async Task<Result<MovieActor, MovieActorException>> CreateEntity(MovieActor entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieActorUnknownException(entity.MovieId, entity.ActorId, exception);
        }
    }
}
