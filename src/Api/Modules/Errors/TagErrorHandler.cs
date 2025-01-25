using Application.Tags.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class TagErrorHandler
{
    public static ObjectResult ToObjectResult(this TagException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            TagUnknownException => StatusCodes.Status400BadRequest,
            TagNotFoundException => StatusCodes.Status404NotFound,
            TagNameAlreadyExistsException => StatusCodes.Status409Conflict,
            TagHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
