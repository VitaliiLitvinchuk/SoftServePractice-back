using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Sessions.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Movies;
using Domain.Sessions;
using Domain.Statuses;
using MediatR;

namespace Application.Sessions.Commands;

public class CreateSessionCommand : IRequest<Result<Session, SessionException>>
{
    public required DateTime StartAt { get; init; }
    public DateTime? EndAt { get; init; }
    public required Guid MovieId { get; init; }
    public required Guid HallId { get; init; }
}

public class CreateSessionCommandHandler(IBaseRepository<Session> repository, IBaseQuery<Session> query, IBaseQuery<Hall> hallQuery, IBaseQuery<Movie> movieQuery, IBaseQuery<Status> statusQuery) : IRequestHandler<CreateSessionCommand, Result<Session, SessionException>>
{
    public async Task<Result<Session, SessionException>> Handle(CreateSessionCommand request, CancellationToken cancellation)
    {
        var id = SessionId.New();
        var movieId = new MovieId(request.MovieId);

        var result = await movieQuery.Get(cancellation, x => x.Id == movieId);

        return await result.Match(
            async movie =>
            {
                var hallId = new HallId(request.HallId);

                var result = await hallQuery.Get(cancellation, x => x.Id == hallId);

                return await result.Match(
                    async hall =>
                    {
                        DateTime endAt = request.EndAt ?? request.StartAt.AddSeconds(movie.Duration).AddMinutes(Defaults.BreakAfterSession + Defaults.BreakAfterSession);

                        endAt = endAt.ToUniversalTime();

                        if (endAt < request.StartAt.AddSeconds(movie.Duration).ToUniversalTime())
                            return await Task.FromResult<Result<Session, SessionException>>(new SessionCannotBeShorterThanMovieDurationException(id));

                        DateTime startAt = request.StartAt.ToUniversalTime();

                        var result = await query.Get(cancellation, x => x.HallId == hallId && x.MovieId == movieId
                           && (x.StartAt >= startAt && x.StartAt <= endAt
                            || x.EndAt >= startAt && x.EndAt <= endAt));

                        return await result.Match(
                            entity => Task.FromResult<Result<Session, SessionException>>(new HallWillHaveSessionInThisTimeException(id, hallId)),
                            async () =>
                            {
                                var status = await statusQuery.Get(cancellation, x => x.Name == Defaults.StatusPending);

                                return await status.Match(
                                    async status =>
                                    {
                                        var entity = Session.New(id, startAt, endAt, status.Id, movie.Id, hall.Id);

                                        return await CreateEntity(entity, cancellation);
                                    },
                                    () => Task.FromResult<Result<Session, SessionException>>(new StatusForSessionNotFoundException(id, StatusId.New()))
                                );
                            }
                        );
                    },
                    () => Task.FromResult<Result<Session, SessionException>>(new HallForSessionNotFoundException(id, hallId))
                );
            },
            () => Task.FromResult<Result<Session, SessionException>>(new MovieForSessionNotFoundException(id, movieId))
        );
    }

    private async Task<Result<Session, SessionException>> CreateEntity(Session entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SessionUnknownException(entity.Id, exception);
        }
    }
}
