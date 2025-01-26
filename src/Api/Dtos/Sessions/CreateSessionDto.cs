namespace Api.Dtos.Sessions;

public record CreateSessionDto(DateTime StartAt, DateTime? EndAt, Guid HallId, Guid MovieId);
