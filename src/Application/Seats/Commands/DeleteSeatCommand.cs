using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Seats.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Seats;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Seats.Commands;

public class DeleteSeatCommand : IRequest<Result<Seat, SeatException>>
{
    public required Guid Id { get; init; }
}

public class DeleteSeatCommandHandler(IBaseRepository<Seat> repository, IBaseQuery<Seat> query) : IRequestHandler<DeleteSeatCommand, Result<Seat, SeatException>>
{
    public async Task<Result<Seat, SeatException>> Handle(DeleteSeatCommand request, CancellationToken cancellation)
    {
        var id = new SeatId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Tickets));

        return await result.Match(
            async entity =>
            {
                if (entity.Tickets.Count != 0)
                    return new SeatHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Seat, SeatException>>(new SeatNotFoundException(id))
        );
    }

    private async Task<Result<Seat, SeatException>> DeleteEntity(Seat entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SeatUnknownException(entity.Id, exception);
        }
    }
}
