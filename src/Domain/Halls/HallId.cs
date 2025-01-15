namespace Domain.Halls;

public record HallId(Guid Value)
{
    public static HallId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
