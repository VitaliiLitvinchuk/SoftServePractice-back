using Domain.Seats;
using Domain.Sessions;

namespace Domain.Halls;

public class Hall(HallId id, string name, short capacity)
{
    public HallId Id { get; } = id;
    public string Name { get; private set; } = name;
    public short Capacity { get; private set; } = capacity;

    public ICollection<Seat> Seats = [];
    public ICollection<Session> Sessions = [];

    public void UpdateDatails(string name, short capacity)
    {
        Name = name;
        Capacity = capacity;
    }

    public static Hall New(HallId id, string name, short capacity)
        => new(id, name, capacity);
}
