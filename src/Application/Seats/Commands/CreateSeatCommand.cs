using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Seats.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Seats;
using MediatR;

namespace Application.Seats.Commands;

public class CreateSeatCommand : IRequest<Result<Seat, SeatException>>
{
    public required int Row { get; init; }
    public required int Number { get; init; }
    public required Guid HallId { get; init; }
}

public class CreateSeatCommandHandler(IBaseRepository<Seat> repository, IBaseQuery<Seat> query, IBaseQuery<Hall> hallQuery) : IRequestHandler<CreateSeatCommand, Result<Seat, SeatException>>
{
    public async Task<Result<Seat, SeatException>> Handle(CreateSeatCommand request, CancellationToken cancellation)
    {
        var id = SeatId.New();
        var hallId = new HallId(request.HallId);
        var entity = Seat.New(id, request.Row, request.Number, hallId);

        var result = await hallQuery.Get(cancellation, x => x.Id == hallId);

        return await result.Match(
            async hall =>
            {
                var result = await query.Get(cancellation, x => x.Row == entity.Row && x.Number == entity.Number && x.HallId == entity.HallId);

                return await result.Match(
                    entity => Task.FromResult<Result<Seat, SeatException>>(new SeatAlreadyExistsException(entity.Id, entity.Row, entity.Number)),
                    async () => await CreateEntity(entity, cancellation)
                );
            },
            () => Task.FromResult<Result<Seat, SeatException>>(new HallForSeatNotFoundException(entity.Id, hallId))
        );
    }

    private async Task<Result<Seat, SeatException>> CreateEntity(Seat entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SeatUnknownException(entity.Id, exception);
        }
    }
}
