using Domain.Halls;
using Domain.Seats;

namespace Application.Seats.Exceptions;

public class SeatException(SeatId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public SeatId Id { get; } = id;
}

public class SeatNotFoundException(SeatId id) : SeatException(id, $"Seat {id} not found.");
public class SeatAlreadyExistsException(SeatId id, int row, int number) : SeatException(id, $"Seat {row}-{number} already exists.");
public class SeatUnknownException(SeatId id, Exception innerException) : SeatException(id, $"Seat {id} is unknown.", innerException);
public class HallForSeatNotFoundException(SeatId id, HallId hallId) : SeatException(id, $"Hall {hallId} for seat not found.");
public class HallIsFullException(SeatId id, HallId hallId) : SeatException(id, $"Hall {hallId} is full.");
public class SeatHasReleationsException(SeatId id) : SeatException(id, $"Seat {id} has relations.");
