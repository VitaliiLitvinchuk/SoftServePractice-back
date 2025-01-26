using Api.Dtos.Seats;
using Api.Dtos.Sessions;
using Domain.Tickets;

namespace Api.Dtos.Tickets;

public record TicketDto(Guid Id, Guid SessionId, Guid SeatId, decimal Price, SessionDto? Session, SeatDto? Seat)
{
    public static TicketDto FromDomainModel(Ticket ticket)
        => new(ticket.Id.Value, ticket.SessionId.Value, ticket.SeatId.Value, ticket.Price,
            ticket.Session is null ? null : SessionDto.FromDomainModel(ticket.Session),
            ticket.Seat is null ? null : SeatDto.FromDomainModel(ticket.Seat));
}
