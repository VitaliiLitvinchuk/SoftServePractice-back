using Domain.Tickets;
using Domain.Users;

namespace Domain.PurchaseHistories;

// TODO: Use a dynamic type for the relation ID of purchase history, possibly through JSON or another method
public class PurchaseHistory(PurchaseHistoryId id, UserId userId, TicketId ticketId, DateTime purchasedAt)
{
    public PurchaseHistoryId Id { get; } = id;
    public DateTime PurchasedAt { get; } = purchasedAt;

    public UserId UserId { get; } = userId;
    public TicketId TicketId { get; } = ticketId;

    public User? User { get; }
    public Ticket? Ticket { get; }

    public static PurchaseHistory New(PurchaseHistoryId id, UserId userId, TicketId movieId, DateTime purchasedAt)
        => new(id, userId, movieId, purchasedAt);
}
