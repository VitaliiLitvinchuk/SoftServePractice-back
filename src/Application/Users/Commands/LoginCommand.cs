using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Application.Users.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands;

public class LoginCommand : IRequest<Result<User, UserException>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginCommandHandler(IBaseQuery<User> query, IHashService hashService) : IRequestHandler<LoginCommand, Result<User, UserException>>
{
    public Task<Result<User, UserException>> Handle(LoginCommand request, CancellationToken cancellation)
    {
        return query.Get(cancellation, x => x.Email == request.Email, include: x => x.Include(x => x.Role)!)
            .ContinueWith(task => task.Result.Match<Result<User, UserException>>(
                user =>
                {
                    var password = hashService.HashPassword(request.Password);

                    if (user.PasswordHash != password)
                        return new UserInvalidDataException(user.Id);

                    return user;
                },
                () => new UserInvalidDataException(UserId.New())
            ));
    }
}
