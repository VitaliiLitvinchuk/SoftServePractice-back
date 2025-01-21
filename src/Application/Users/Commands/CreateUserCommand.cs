using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Users.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class CreateUserCommandHandler(
    IBaseRepository<User> repository,
    IBaseQuery<User> query,
    IBaseQuery<Role> roles,
    IHashService hashService
    ) : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request, CancellationToken cancellation)
    {
        var id = UserId.New();

        var result = await query.Get(cancellation, x => x.Email == request.Email);

        return await result.Match(
            entity => Task.FromResult<Result<User, UserException>>(new UserEmailAlreadyExistsException(entity.Id, entity.Email)),
            async () =>
            {
                var result = await roles.Get(cancellation, x => x.Name == Defaults.UserRole);

                return await result.Match(
                    async role =>
                    {
                        var user = User.New(id, request.Email, hashService.HashPassword(request.Password), role.Id);

                        return await CreateEntity(user, cancellation);
                    },
                    () => Task.FromResult<Result<User, UserException>>(new RoleForUserNotFoundException(id, null))
                );
            }
        );
    }

    public async Task<Result<User, UserException>> CreateEntity(User entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}
