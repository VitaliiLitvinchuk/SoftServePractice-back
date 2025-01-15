namespace Domain.Tags;

public class Tag(TagId tagId, string name)
{
    public TagId Id { get; } = tagId;
    public string Name { get; private set; } = name;

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Tag New(TagId tagId, string name)
        => new(tagId, name);
}
