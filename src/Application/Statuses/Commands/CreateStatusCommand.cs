using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public class CreateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required string Name { get; init; }
}

public class CreateStatusCommandHandler(IBaseRepository<Status> repository, IBaseQuery<Status> query) : IRequestHandler<CreateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(CreateStatusCommand request, CancellationToken cancellation)
    {
        var id = StatusId.New();
        var entity = Status.New(id, request.Name);

        var result = await query.Get(cancellation, x => x.Name == entity.Name);

        return await result.Match(
            entity => Task.FromResult<Result<Status, StatusException>>(new StatusNameAlreadyExistsException(entity.Id, entity.Name)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    private async Task<Result<Status, StatusException>> CreateEntity(Status entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}
