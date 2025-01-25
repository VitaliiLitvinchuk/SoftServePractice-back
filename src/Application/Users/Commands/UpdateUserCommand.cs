using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public class UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid Id { get; init; }
    public required Guid RoleId { get; init; }
}

public class UpdateUserCommandHandler(IBaseRepository<User> repository, IBaseQuery<User> query, IBaseQuery<Role> roles) : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request, CancellationToken cancellation)
    {
        var id = new UserId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Role)!);

        return await result.Match(
            async entity =>
            {
                var roleId = new RoleId(request.RoleId);
                var result = await roles.Get(cancellation, x => x.Id == roleId);

                return await result.Match(
                    async role => await UpdateEntity(entity, role.Id, cancellation),
                    () => Task.FromResult<Result<User, UserException>>(new RoleForUserNotFoundException(id, roleId))
                );
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(id))
        );
    }

    private async Task<Result<User, UserException>> UpdateEntity(User entity, RoleId roleId, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateRole(roleId);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
