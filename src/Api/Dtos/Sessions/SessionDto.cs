using Api.Dtos.Halls;
using Api.Dtos.Movies;
using Api.Dtos.Statuses;
using Domain.Sessions;

namespace Api.Dtos.Sessions;

public record SessionDto(Guid Id, Guid StatusId, Guid MovieId, Guid HallId, DateTime StartAt, DateTime EndAt, StatusDto? Status, MovieDto? Movie, HallDto? Hall)
{
    public static SessionDto FromDomainModel(Session session)
        => new(session.Id.Value, session.StatusId.Value, session.MovieId.Value, session.HallId.Value, session.StartAt, session.EndAt,
            session.Status is null ? null : StatusDto.FromDomainModel(session.Status),
            session.Movie is null ? null : MovieDto.FromDomainModel(session.Movie),
            session.Hall is null ? null : HallDto.FromDomainModel(session.Hall));
}
