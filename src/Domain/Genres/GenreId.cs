namespace Domain.Genres;

public record GenreId(Guid Value)
{
    public static GenreId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
