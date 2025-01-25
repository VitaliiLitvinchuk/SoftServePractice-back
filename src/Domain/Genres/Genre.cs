using Domain.GenresTags;
using Domain.MoviesGenres;

namespace Domain.Genres;

public class Genre(GenreId id, string name)
{
    public GenreId Id { get; } = id;
    public string Name { get; private set; } = name;

    public ICollection<MovieGenre> Movies = [];
    public ICollection<GenreTag> Tags = [];

    public void UpdateDetails(string name)
    {
        Name = name;
    }

    public static Genre New(GenreId id, string name)
        => new(id, name);
}
