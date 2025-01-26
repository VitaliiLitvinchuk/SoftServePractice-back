namespace Api.Dtos.Tickets;

public record CreateTicketDto(Guid SessionId, Guid SeatId, decimal Price);
