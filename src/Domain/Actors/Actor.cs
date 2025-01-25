using Domain.MoviesActors;

namespace Domain.Actors;

public class Actor(ActorId id, string name, string surname, string middlename, string imageUrl)
{
    public ActorId Id { get; } = id;
    public string Name { get; private set; } = name;
    public string Surname { get; private set; } = surname;
    public string Middlename { get; private set; } = middlename;
    public string ImageUrl { get; private set; } = imageUrl;

    public ICollection<MovieActor> Movies { get; } = [];

    public void UpdateDetails(string name, string surname, string middlename, string imageUrl)
    {
        Name = name;
        Surname = surname;
        Middlename = middlename;
        ImageUrl = imageUrl;
    }

    public static Actor New(ActorId id, string name, string surname, string middlename, string imageUrl)
        => new(id, name, surname, middlename, imageUrl);
}
