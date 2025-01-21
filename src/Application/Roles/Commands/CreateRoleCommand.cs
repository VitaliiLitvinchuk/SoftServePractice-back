using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Roles.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using MediatR;

namespace Application.Roles.Commands;

public class CreateRoleCommand : IRequest<Result<Role, RoleException>>
{
    public required string Name { get; init; }
}

public class CreateRoleCommandHandler(IBaseRepository<Role> repository, IBaseQuery<Role> query) : IRequestHandler<CreateRoleCommand, Result<Role, RoleException>>
{
    public async Task<Result<Role, RoleException>> Handle(CreateRoleCommand request, CancellationToken cancellation)
    {
        var id = RoleId.New();
        var entity = Role.New(id, request.Name);

        var result = await query.Get(cancellation, x => x.Name == entity.Name);

        return await result.Match(
            entity => Task.FromResult<Result<Role, RoleException>>(new RoleNameAlreadyExistsException(entity.Id, entity.Name)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    public async Task<Result<Role, RoleException>> CreateEntity(Role entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new RoleUnknownException(entity.Id, exception);
        }
    }
}
