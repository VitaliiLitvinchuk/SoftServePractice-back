using Application.Actors.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using CSharpFunctionalExtensions;
using Domain.Actors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Actors.Commands;

public class DeleteActorCommand : IRequest<Result<Actor, ActorException>>
{
    public required Guid Id { get; init; }
}

public class DeleteActorCommandHandler(IBaseRepository<Actor> repository, IBaseQuery<Actor> query) : IRequestHandler<DeleteActorCommand, Result<Actor, ActorException>>
{
    public async Task<Result<Actor, ActorException>> Handle(DeleteActorCommand request, CancellationToken cancellation)
    {
        var id = new ActorId(request.Id);

        var result = await query.Get(filter: x => x.Id == id, include: x => x.Include(x => x.Movies), cancellation: cancellation);

        return await result.Match(
            async actor =>
            {
                if (actor.Movies.Count != 0)
                    return new ActorHasReleationsException(id);

                return await DeleteEntity(actor, cancellation);
            },
            () => Task.FromResult<Result<Actor, ActorException>>(new ActorNotFoundException(id))
        );
    }

    public async Task<Result<Actor, ActorException>> DeleteEntity(Actor entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new ActorUnknownException(entity.Id, exception);
        }
    }
}
