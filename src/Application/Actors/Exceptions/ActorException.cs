using Domain.Actors;

namespace Application.Actors.Exceptions;

public class ActorException(ActorId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public ActorId Id { get; } = id;
}

public class ActorNotFoundException(ActorId id) : ActorException(id, $"Actor {id} not found.");
public class ActorUnknownException(ActorId id, Exception innerException) : ActorException(id, $"Actor {id} is unknown.", innerException);
public class ActorHasReleationsException(ActorId id) : ActorException(id, $"Actor {id} has relations.");