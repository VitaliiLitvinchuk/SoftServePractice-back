using Domain.Halls;
using Domain.Tickets;

namespace Domain.Seats;

public class Seat(SeatId id, int row, int number, HallId hallId)
{
    public SeatId Id { get; } = id;
    public int Row { get; private set; } = row;
    public int Number { get; private set; } = number;

    public HallId HallId { get; } = hallId;
    public Hall? Hall { get; }

    public ICollection<Ticket> Tickets { get; } = [];

    public void UpdateDetails(int row, int number)
    {
        Row = row;
        Number = number;
    }

    public static Seat New(SeatId id, int row, int number, HallId hallId)
        => new(id, row, number, hallId);
}
