namespace Domain.Actors;

public record ActorId(Guid Value)
{
    public static ActorId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
