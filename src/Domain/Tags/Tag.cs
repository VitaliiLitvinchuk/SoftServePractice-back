using Domain.GenresTags;
using Domain.MoviesTags;

namespace Domain.Tags;

public class Tag(TagId id, string name)
{
    public TagId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<GenreTag> Genres { get; } = [];
    public ICollection<MovieTag> Movies { get; } = [];

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Tag New(TagId id, string name)
        => new(id, name);
}
