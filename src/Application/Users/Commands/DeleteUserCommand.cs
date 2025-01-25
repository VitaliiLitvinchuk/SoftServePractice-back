using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public class DeleteUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; init; }
}

public class DeleteUserCommandHandler(IBaseRepository<User> repository, IBaseQuery<User> query) : IRequestHandler<DeleteUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(DeleteUserCommand request, CancellationToken cancellation)
    {
        var id = new UserId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.MovieRatings).Include(x => x.PurchaseHistories));

        return await result.Match(
            async entity =>
            {
                if (entity.MovieRatings.Count != 0)
                    return new UserHasReleationsException(id);

                if (entity.PurchaseHistories.Count != 0)
                    return new UserHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(id))
        );
    }

    private async Task<Result<User, UserException>> DeleteEntity(User entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
