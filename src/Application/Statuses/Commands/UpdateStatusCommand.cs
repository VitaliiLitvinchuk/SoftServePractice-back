using System;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Statuses;
using MediatR;

namespace Application.Statuses.Commands;

public class UpdateStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateStatusCommandHandler(IBaseRepository<Status> repository, IBaseQuery<Status> query) : IRequestHandler<UpdateStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(UpdateStatusCommand request, CancellationToken cancellation)
    {
        var id = new StatusId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
                var result = await query.Get(cancellation, x => x.Name == request.Name);

                return await result.Match(
                  entity => Task.FromResult<Result<Status, StatusException>>(new StatusNameAlreadyExistsException(entity.Id, entity.Name)),
                  async () => await UpdateEntity(entity, request.Name, cancellation)
                );
            },
            () => Task.FromResult<Result<Status, StatusException>>(new StatusNotFoundException(id))
        );
    }

    private async Task<Result<Status, StatusException>> UpdateEntity(Status entity, string name, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}
