using Application.Actors.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using CSharpFunctionalExtensions;
using Domain.Actors;
using MediatR;

namespace Application.Actors.Commands;

public class UpdateActorCommand : IRequest<Result<Actor, ActorException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Surname { get; init; }
    public required string Middlename { get; init; }
    public required string ImageUrl { get; init; }
}

public class UpdateActorCommandHandler(IBaseRepository<Actor> repository, IBaseQuery<Actor> query) : IRequestHandler<UpdateActorCommand, Result<Actor, ActorException>>
{
    public async Task<Result<Actor, ActorException>> Handle(UpdateActorCommand request, CancellationToken cancellation)
    {
        var id = new ActorId(request.Id);
        var result = await query.Get(filter: x => x.Id == id, cancellation: cancellation);

        return await result.Match(
            async entity => await UpdateEntity(entity, request.Name, request.Surname, request.Middlename, request.ImageUrl, cancellation),
            () => Task.FromResult<Result<Actor, ActorException>>(new ActorNotFoundException(id))
        );
    }

    private async Task<Result<Actor, ActorException>> UpdateEntity(Actor entity, string name, string surname, string middlename, string imageUrl, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name, surname, middlename, imageUrl);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new ActorUnknownException(entity.Id, exception);
        }
    }
}
