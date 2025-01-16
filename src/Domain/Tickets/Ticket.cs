using Domain.Seats;
using Domain.Sessions;

namespace Domain.Tickets;

public class Ticket(SessionId sessionId, decimal price, SeatId seatId)
{
    public SessionId SessionId { get; } = sessionId;
    public SeatId SeatId { get; } = seatId;

    public decimal Price { get; private set; } = price;

    public Session? Session { get; }
    public Seat? Seat { get; }

    public void UpdateDetails(decimal price)
    {
        Price = price;
    }

    public static Ticket New(SessionId sessionId, decimal price, SeatId seatId)
        => new(sessionId, price, seatId);
}
