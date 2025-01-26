using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tickets.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Seats;
using Domain.Sessions;
using Domain.Tickets;
using MediatR;

namespace Application.Tickets.Commands;

public class CreateTicketCommand : IRequest<Result<Ticket, TicketException>>
{
    public required Guid SessionId { get; init; }
    public required Guid SeatId { get; init; }
    public required decimal Price { get; init; }
}

public class CreateTicketCommandHandler(IBaseRepository<Ticket> repository, IBaseQuery<Ticket> query, IBaseQuery<Session> querySession, IBaseQuery<Seat> querySeat) : IRequestHandler<CreateTicketCommand, Result<Ticket, TicketException>>
{
    public async Task<Result<Ticket, TicketException>> Handle(CreateTicketCommand request, CancellationToken cancellation)
    {
        var id = TicketId.New();
        var sessionId = new SessionId(request.SessionId);

        var result = await querySession.Get(cancellation, x => x.Id == sessionId);

        return await result.Match(
            async entity =>
            {
                var seatId = new SeatId(request.SeatId);

                var result = await querySeat.Get(cancellation, x => x.Id == seatId);

                return await result.Match(
                    async entity =>
                    {
                        var result = await query.Get(cancellation, x => x.SessionId == sessionId && x.SeatId == seatId);

                        return await result.Match(
                            entity => Task.FromResult<Result<Ticket, TicketException>>(new TicketAlreadyExistsException(id, sessionId, seatId)),
                            async () =>
                            {
                                var ticket = Ticket.New(id, sessionId, request.Price, seatId);

                                return await CreateEntity(ticket, cancellation);
                            }
                        );
                    },
                    () => Task.FromResult<Result<Ticket, TicketException>>(new SeatForTicketNotFoundException(id, seatId))
                );
            },
            () => Task.FromResult<Result<Ticket, TicketException>>(new SessionForTicketNotFoundException(id, sessionId))
        );
    }

    private async Task<Result<Ticket, TicketException>> CreateEntity(Ticket entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TicketUnknownException(entity.Id, exception);
        }
    }
}
