using Domain.Halls;

namespace Domain.Seats;

public class Seat(SeatId seatId, int row, int number, HallId hallId)
{
    public SeatId Id { get; } = seatId;
    public int Row { get; private set; } = row;
    public int Number { get; private set; } = number;

    public HallId HallId { get; } = hallId;
    public Hall? Hall { get; }

    public void UpdateDatails(int row, int number)
    {
        Row = row;
        Number = number;
    }

    public static Seat New(SeatId seatId, int row, int number, HallId hallId)
        => new(seatId, row, number, hallId);
}
