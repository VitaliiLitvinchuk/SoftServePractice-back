using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using MediatR;

namespace Application.Roles.Commands;

public class UpdateRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateRoleCommandHandler(IBaseRepository<Role> repository, IBaseQuery<Role> query) : IRequestHandler<UpdateRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(UpdateRoleCommand request, CancellationToken cancellation)
    {
        var id = new RoleId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
                if (entity.Name != request.Name)
                {
                    var result = await query.GetMany(cancellation, x => x.Name == request.Name);

                    if (result.Any())
                    {
                        return new RoleNameAlreadyExistsException(id, request.Name);
                    }
                }

                return await UpdateEntity(entity, request.Name, cancellation);
            },
            () => Task.FromResult<Result<Role, RoleException>>(new RoleNotFoundException(id))
        );
    }

    private async Task<Result<Role, RoleException>> UpdateEntity(Role entity, string name, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(entity.Id, exception);
        }
    }
}
