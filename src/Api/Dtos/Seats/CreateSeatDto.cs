namespace Api.Dtos.Seats;

public record CreateSeatDto(int Row, int Number, Guid HallId);
