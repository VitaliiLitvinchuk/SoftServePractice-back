using Domain.Seats;
using Domain.Sessions;
using Domain.Tickets;

namespace Application.Tickets.Exceptions;

public class TicketException(TicketId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public TicketId Id { get; } = id;
}

public class TicketNotFoundException(TicketId id) : TicketException(id, $"Ticket {id} not found.");
public class TicketHasReleationsException(TicketId id) : TicketException(id, $"Ticket {id} has relations.");
public class SessionForTicketNotFoundException(TicketId id, SessionId sessionId) : TicketException(id, $"Session {sessionId} for ticket not found.");
public class SeatForTicketNotFoundException(TicketId id, SeatId seatId) : TicketException(id, $"Seat {seatId} for ticket not found.");
public class TicketAlreadyExistsException(TicketId id, SessionId sessionId, SeatId seatId) : TicketException(id, $"Ticket for session {sessionId} and seat {seatId} already exists.");
public class TicketUnknownException(TicketId id, Exception innerException) : TicketException(id, $"Ticket {id} is unknown.", innerException);