namespace Domain.Tags;

public record TagId(Guid Value)
{
    public static TagId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
