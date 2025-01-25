namespace Api.Dtos.Seats;

public record UpdateSeatDto(Guid Id, int Row, int Number, Guid HallId);
