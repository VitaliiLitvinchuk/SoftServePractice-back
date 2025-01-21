using Application.Actors.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ActorErrorHandler
{
    public static ObjectResult ToObjectResult(this ActorException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            ActorUnknownException => StatusCodes.Status400BadRequest,
            ActorNotFoundException => StatusCodes.Status404NotFound,
            ActorHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
