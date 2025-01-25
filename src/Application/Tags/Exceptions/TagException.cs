using Domain.Tags;

namespace Application.Tags.Exceptions;

public class TagException(TagId id, string message, Exception? innerException = null) : Exception(message, innerException)
{
    public TagId Id { get; } = id;
}

public class TagNotFoundException(TagId id) : TagException(id, $"Tag {id} not found.");
public class TagNameAlreadyExistsException(TagId id, string name) : TagException(id, $"Tag with name {name} already exists.");
public class TagUnknownException(TagId id, Exception innerException) : TagException(id, $"Tag {id} is unknown.", innerException);
public class TagHasReleationsException(TagId id) : TagException(id, $"Tag {id} has relations.");