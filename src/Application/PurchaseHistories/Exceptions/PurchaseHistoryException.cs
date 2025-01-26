using Domain.PurchaseHistories;
using Domain.Tickets;
using Domain.Users;

namespace Application.PurchaseHistories.Exceptions;

public class PurchaseHistoryException(PurchaseHistoryId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public PurchaseHistoryId Id { get; } = id;
}

public class PurchaseHistoryNotFoundException(PurchaseHistoryId id) : PurchaseHistoryException(id, $"PurchaseHistory {id} not found.");
public class PurchaseHistoryAlreadyExistsException(PurchaseHistoryId id) : PurchaseHistoryException(id, $"PurchaseHistory {id} already exists.");
public class UserForPurchaseHistoryNotFoundException(PurchaseHistoryId id, UserId userId) : PurchaseHistoryException(id, $"User {userId} for purchase history not found.");
public class TicketForPurchaseHistoryNotFoundException(PurchaseHistoryId id, TicketId ticketId) : PurchaseHistoryException(id, $"Ticket {ticketId} for purchase history not found.");
public class PurchaseHistoryUnknownException(PurchaseHistoryId id, Exception innerException) : PurchaseHistoryException(id, $"PurchaseHistory {id} is unknown.", innerException);