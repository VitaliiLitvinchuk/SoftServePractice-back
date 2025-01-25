using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Seats.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Seats;
using MediatR;

namespace Application.Seats.Commands;

public class UpdateSeatCommand : IRequest<Result<Seat, SeatException>>
{
    public required Guid Id { get; init; }
    public required int Row { get; init; }
    public required int Number { get; init; }
    public required Guid HallId { get; init; }
}

public class UpdateSeatCommandHandler(IBaseRepository<Seat> repository, IBaseQuery<Seat> query, IBaseQuery<Hall> hallQuery) : IRequestHandler<UpdateSeatCommand, Result<Seat, SeatException>>
{
    public async Task<Result<Seat, SeatException>> Handle(UpdateSeatCommand request, CancellationToken cancellation)
    {
        var id = new SeatId(request.Id);
        var hallId = new HallId(request.HallId);

        var result = await hallQuery.Get(cancellation, x => x.Id == hallId);

        return await result.Match(
            async entity =>
            {
                var result = await query.Get(cancellation, x => x.Id == id);

                return await result.Match(
                    async entity => await UpdateEntity(entity, request.Row, request.Number, cancellation),
                    () => Task.FromResult<Result<Seat, SeatException>>(new SeatNotFoundException(id))
                );
            },
            () => Task.FromResult<Result<Seat, SeatException>>(new HallForSeatNotFoundException(id, hallId))
        );
    }

    private async Task<Result<Seat, SeatException>> UpdateEntity(Seat entity, int row, int number, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(row, number);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SeatUnknownException(entity.Id, exception);
        }
    }
}
