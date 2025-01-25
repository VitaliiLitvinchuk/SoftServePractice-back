using Domain.Sessions;

namespace Domain.Statuses;

public class Status(StatusId id, string name)
{
    public StatusId Id { get; } = id;
    public string Name { get; set; } = name;

    public ICollection<Session> Sessions { get; } = [];

    public void UpdateDetails(string name)
    {
        Name = name;
    }

    public static Status New(StatusId id, string name)
        => new(id, name);
}
