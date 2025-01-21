using Domain.Roles;

namespace Application.Roles.Exceptions;

public class RoleException(RoleId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public RoleId Id { get; } = id;
}

public class RoleNotFoundException(RoleId id) : RoleException(id, $"Role {id} not found.");
public class RoleNameAlreadyExistsException(RoleId id, string name) : RoleException(id, $"Role with name {name} already exists.");
public class RoleUnknownException(RoleId id, Exception innerException) : RoleException(id, $"Role {id} is unknown.", innerException);
public class RoleHasReleationsException(RoleId id) : RoleException(id, $"Role {id} has relations.");