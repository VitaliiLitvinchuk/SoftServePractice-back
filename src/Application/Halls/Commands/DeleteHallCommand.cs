using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Halls.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Halls.Commands;

public class DeleteHallCommand : IRequest<Result<Hall, HallException>>
{
    public required Guid Id { get; init; }
}

public class DeleteHallCommandHandler(IBaseRepository<Hall> repository, IBaseQuery<Hall> query) : IRequestHandler<DeleteHallCommand, Result<Hall, HallException>>
{
    public async Task<Result<Hall, HallException>> Handle(DeleteHallCommand request, CancellationToken cancellation)
    {
        var id = new HallId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Sessions).Include(x => x.Seats));

        return await result.Match(
            async entity =>
            {
                if (entity.Sessions.Count != 0)
                    return new HallHasReleationsException(id);

                if (entity.Seats.Count != 0)
                    return new HallHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Hall, HallException>>(new HallNotFoundException(id))
        );
    }

    private async Task<Result<Hall, HallException>> DeleteEntity(Hall entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new HallUnknownException(entity.Id, exception);
        }
    }
}
