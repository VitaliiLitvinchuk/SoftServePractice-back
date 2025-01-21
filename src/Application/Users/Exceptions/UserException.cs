using Domain.Roles;
using Domain.Users;

namespace Application.Users.Exceptions;

public class UserException(UserId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public UserId Id { get; } = id;
}

public class UserInvalidDataException(UserId id) : UserException(id, $"User email or password is incorrect.");
public class UserNotFoundException(UserId id) : UserException(id, $"User {id} not found.");
public class UserEmailAlreadyExistsException(UserId id, string email) : UserException(id, $"User with email {email} already exists.");
public class RoleForUserNotFoundException(UserId userId, RoleId? roleId = null) : UserException(userId, $"Role{(roleId is not null ? $" {roleId}" : "")} for user {userId} not found.");
public class UserUnknownException(UserId id, Exception innerException) : UserException(id, $"User {id} is unknown.", innerException);
public class UserHasReleationsException(UserId id) : UserException(id, $"User {id} has relations.");