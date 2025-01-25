using Domain.Halls;

namespace Application.Halls.Exceptions;

public class HallException(HallId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public HallId Id { get; } = id;
}

public class HallNotFoundException(HallId id) : HallException(id, $"Hall {id} not found.");
public class HallNameAlreadyExistsException(HallId id, string name) : HallException(id, $"Hall with name {name} already exists.");
public class HallUnknownException(HallId id, Exception innerException) : HallException(id, $"Hall {id} is unknown.", innerException);
public class HallHasReleationsException(HallId id) : HallException(id, $"Hall {id} has relations.");
