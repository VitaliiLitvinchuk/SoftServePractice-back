using Domain.GenresTags;
using Domain.MoviesGenres;

namespace Domain.Genres;

public class Genre(GenreId genreId, string name)
{
    public GenreId Id { get; } = genreId;
    public string Name { get; private set; } = name;

    public ICollection<MovieGenre> MovieGenres = [];
    public ICollection<GenreTag> GenreTags = [];

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Genre New(GenreId genreId, string name)
        => new(genreId, name);
}
