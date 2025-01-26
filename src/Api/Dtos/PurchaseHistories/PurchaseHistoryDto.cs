using Api.Dtos.Tickets;
using Api.Dtos.Users;
using Domain.PurchaseHistories;

namespace Api.Dtos.PurchaseHistories;

public record PurchaseHistoryDto(Guid Id, Guid UserId, Guid TicketId, DateTime PurchasedAt, UserDto? User, TicketDto? Ticket)
{
    public static PurchaseHistoryDto FromDomainModel(PurchaseHistory purchaseHistory)
        => new(purchaseHistory.Id.Value, purchaseHistory.UserId.Value, purchaseHistory.TicketId.Value, purchaseHistory.PurchasedAt,
            purchaseHistory.User is null ? null : UserDto.FromDomainModel(purchaseHistory.User),
            purchaseHistory.Ticket is null ? null : TicketDto.FromDomainModel(purchaseHistory.Ticket));
}
