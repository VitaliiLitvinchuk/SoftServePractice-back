using Application.Actors.Exceptions;
using Application.Common.Interfaces.Repositories;
using CSharpFunctionalExtensions;
using Domain.Actors;
using MediatR;

namespace Application.Actors.Commands;

public class CreateActorCommand : IRequest<Result<Actor, ActorException>>
{
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Middlename { get; init; }
    public required string ImageUrl { get; init; }
}

public class CreateActorCommandHandler(IBaseRepository<Actor> repository) : IRequestHandler<CreateActorCommand, Result<Actor, ActorException>>
{
    public async Task<Result<Actor, ActorException>> Handle(CreateActorCommand request, CancellationToken cancellation)
    {
        var id = ActorId.New();
        var entity = Actor.New(id, request.Name, request.Surname, request.Middlename, request.ImageUrl);

        return await CreateEntity(entity, cancellation);
    }

    private async Task<Result<Actor, ActorException>> CreateEntity(Actor entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new ActorUnknownException(entity.Id, exception);
        }
    }
}
