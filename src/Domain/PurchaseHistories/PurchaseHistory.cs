using Domain.Movies;
using Domain.Users;

namespace Domain.PurchaseHistories;

// TODO: Use a dynamic type for the relation ID of purchase history, possibly through JSON or another method
public class PurchaseHistory(PurchaseHistoryId purchaseHistoryId, UserId userId, MovieId movieId, DateTime purchasedAt)
{
    public PurchaseHistoryId Id { get; } = purchaseHistoryId;
    public UserId UserId { get; } = userId;
    public MovieId MovieId { get; } = movieId;
    public DateTime PurchasedAt { get; } = purchasedAt;

    public static PurchaseHistory New(PurchaseHistoryId purchaseHistoryId, UserId userId, MovieId movieId, DateTime purchasedAt)
        => new(purchaseHistoryId, userId, movieId, purchasedAt);
}
