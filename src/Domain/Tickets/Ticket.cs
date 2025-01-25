using Domain.PurchaseHistories;
using Domain.Seats;
using Domain.Sessions;

namespace Domain.Tickets;

public class Ticket(TicketId id, SessionId sessionId, decimal price, SeatId seatId)
{
    public TicketId Id { get; } = id;
    public SessionId SessionId { get; } = sessionId;
    public SeatId SeatId { get; } = seatId;

    public decimal Price { get; private set; } = price;

    public Session? Session { get; }
    public Seat? Seat { get; }

    public ICollection<PurchaseHistory> PurchaseHistories { get; } = [];

    public void UpdateDetails(decimal price)
    {
        Price = price;
    }

    public static Ticket New(TicketId id, SessionId sessionId, decimal price, SeatId seatId)
        => new(id, sessionId, price, seatId);
}
