using Domain.Sessions;

namespace Domain.Statuses;

public class Status(StatusId statusId, string name)
{
    public StatusId Id { get; } = statusId;
    public string Name { get; set; } = name;

    public ICollection<Session> Sessions { get; } = [];

    public void UpdateDatails(string name)
    {
        Name = name;
    }

    public static Status New(StatusId statusId, string name)
        => new(statusId, name);
}
