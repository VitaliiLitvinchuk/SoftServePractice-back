namespace Domain.Seats;

public record SeatId(Guid Value)
{
    public static SeatId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
