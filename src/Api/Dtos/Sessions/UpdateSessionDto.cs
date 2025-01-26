namespace Api.Dtos.Sessions;

public record UpdateSessionDto(Guid Id, DateTime StartAt, DateTime EndAt, Guid StatusId, Guid MovieId, Guid HallId);