using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Sessions.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Sessions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Sessions.Commands;

public class DeleteSessionCommand : IRequest<Result<Session, SessionException>>
{
    public required Guid Id { get; init; }
}

public class DeleteSessionCommandHandler(IBaseRepository<Session> repository, IBaseQuery<Session> query) : IRequestHandler<DeleteSessionCommand, Result<Session, SessionException>>
{
    public async Task<Result<Session, SessionException>> Handle(DeleteSessionCommand request, CancellationToken cancellation)
    {
        var id = new SessionId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Tickets));

        return await result.Match(
            async entity =>
            {
                if (entity.Tickets.Count != 0)
                    return new SessionHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Session, SessionException>>(new SessionNotFoundException(id))
        );
    }

    private async Task<Result<Session, SessionException>> DeleteEntity(Session entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new SessionUnknownException(entity.Id, exception);
        }
    }
}
