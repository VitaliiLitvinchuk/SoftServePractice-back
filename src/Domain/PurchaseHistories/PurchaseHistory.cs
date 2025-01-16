using Domain.Movies;
using Domain.Users;

namespace Domain.PurchaseHistories;

// TODO: Use a dynamic type for the relation ID of purchase history, possibly through JSON or another method
public class PurchaseHistory(PurchaseHistoryId id, UserId userId, MovieId movieId, DateTime purchasedAt)
{
    public PurchaseHistoryId Id { get; } = id;
    public DateTime PurchasedAt { get; } = purchasedAt;

    public UserId UserId { get; } = userId;
    public MovieId MovieId { get; } = movieId;

    public User? User { get; }
    public Movie? Movie { get; }

    public static PurchaseHistory New(PurchaseHistoryId id, UserId userId, MovieId movieId, DateTime purchasedAt)
        => new(id, userId, movieId, purchasedAt);
}
