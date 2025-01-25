using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.Commands;

public class DeleteRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required Guid Id { get; init; }
}

public class DeleteRoleCommandHandler(IBaseRepository<Role> repository, IBaseQuery<Role> query) : IRequestHandler<DeleteRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(DeleteRoleCommand request, CancellationToken cancellation)
    {
        var id = new RoleId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Users));

        return await result.Match(
            async entity =>
            {
                if (entity.Users.Count != 0)
                    return new RoleHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Role, RoleException>>(new RoleNotFoundException(id))
        );
    }

    private async Task<Result<Role, RoleException>> DeleteEntity(Role entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(entity.Id, exception);
        }
    }
}
