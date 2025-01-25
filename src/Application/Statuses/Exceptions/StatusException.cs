using Domain.Statuses;

namespace Application.Statuses.Exceptions;

public class StatusException(StatusId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public StatusId Id { get; } = id;
}

public class StatusNotFoundException(StatusId id) : StatusException(id, $"Status {id} not found.");
public class StatusNameAlreadyExistsException(StatusId id, string name) : StatusException(id, $"Status with name {name} already exists.");
public class StatusUnknownException(StatusId id, Exception innerException) : StatusException(id, $"Status {id} is unknown.", innerException);
public class StatusHasReleationsException(StatusId id) : StatusException(id, $"Status {id} has relations.");