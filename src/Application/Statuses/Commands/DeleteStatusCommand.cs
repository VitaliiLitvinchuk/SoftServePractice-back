using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Statuses.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Statuses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Statuses.Commands;

public class DeleteStatusCommand : IRequest<Result<Status, StatusException>>
{
    public required Guid Id { get; init; }
}

public class DeleteStatusCommandHandler(IBaseRepository<Status> repository, IBaseQuery<Status> query) : IRequestHandler<DeleteStatusCommand, Result<Status, StatusException>>
{
    public async Task<Result<Status, StatusException>> Handle(DeleteStatusCommand request, CancellationToken cancellation)
    {
        var id = new StatusId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Sessions));

        return await result.Match(
            async entity =>
            {
                if (entity.Sessions.Count != 0)
                    return new StatusHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Status, StatusException>>(new StatusNotFoundException(id))
        );
    }

    private async Task<Result<Status, StatusException>> DeleteEntity(Status entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new StatusUnknownException(entity.Id, exception);
        }
    }
}
