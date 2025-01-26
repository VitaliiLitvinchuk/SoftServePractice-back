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

public class UpdateSessionCommand : IRequest<Result<Session, SessionException>>
{
    public required Guid Id { get; init; }
    public required DateTime StartAt { get; init; }
    public required DateTime EndAt { get; init; }
    public required Guid StatusId { get; init; }
    public required Guid MovieId { get; init; }
    public required Guid HallId { get; init; }
}

public class UpdateSessionCommandHandler(IBaseRepository<Session> repository, IBaseQuery<Session> query, IBaseQuery<Hall> hallQuery, IBaseQuery<Movie> movieQuery, IBaseQuery<Status> statusQuery) : IRequestHandler<UpdateSessionCommand, Result<Session, SessionException>>
{
    public async Task<Result<Session, SessionException>> Handle(UpdateSessionCommand request, CancellationToken cancellation)
    {
        var id = new SessionId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
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
                                if (request.EndAt < request.StartAt.AddSeconds(movie.Duration))
                                    return await Task.FromResult<Result<Session, SessionException>>(new SessionCannotBeShorterThanMovieDurationException(id));

                                DateTime endAt = request.EndAt.ToUniversalTime();
                                DateTime startAt = request.StartAt.ToUniversalTime();

                                var result = await query.Get(cancellation, x => x.HallId == hallId && x.MovieId == movieId
                                   && (x.StartAt >= startAt && x.StartAt <= endAt
                                    || x.EndAt >= startAt && x.EndAt <= endAt));

                                return await result.Match(
                                    entity => Task.FromResult<Result<Session, SessionException>>(new HallWillHaveSessionInThisTimeException(id, hallId)),
                                    async () =>
                                    {
                                        var statusId = new StatusId(request.StatusId);

                                        var result = await statusQuery.Get(cancellation, x => x.Id == statusId);

                                        return await result.Match(
                                            async status => await UpdateEntity(entity, startAt, endAt, statusId, movieId, hallId, cancellation),
                                            () => Task.FromResult<Result<Session, SessionException>>(new StatusForSessionNotFoundException(id, statusId))
                                        );
                                    }
                                );
                            },
                            () => Task.FromResult<Result<Session, SessionException>>(new HallForSessionNotFoundException(id, hallId))
                        );
                    },
                    () => Task.FromResult<Result<Session, SessionException>>(new MovieForSessionNotFoundException(id, movieId))
                );
            },
            () => Task.FromResult<Result<Session, SessionException>>(new SessionNotFoundException(id))
        );
    }

    private async Task<Result<Session, SessionException>> UpdateEntity(Session entity, DateTime startAt, DateTime endAt, StatusId statusId, MovieId movieId, HallId hallId, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(startAt, endAt);

            if (entity.StatusId != statusId)
                entity.UpdateStatus(statusId);

            if (entity.MovieId != movieId)
                entity.UpdateMovie(movieId);

            if (entity.HallId != hallId)
                entity.UpdateHall(hallId);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SessionUnknownException(entity.Id, exception);
        }
    }
}
