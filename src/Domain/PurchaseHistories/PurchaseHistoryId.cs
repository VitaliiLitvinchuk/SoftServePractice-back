namespace Domain.PurchaseHistories;

public record PurchaseHistoryId(Guid Value)
{
    public static PurchaseHistoryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
