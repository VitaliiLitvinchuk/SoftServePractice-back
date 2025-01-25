using Application.Statuses.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class StatusErrorHandler
{
    public static ObjectResult ToObjectResult(this StatusException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            StatusUnknownException => StatusCodes.Status400BadRequest,
            StatusNotFoundException => StatusCodes.Status404NotFound,
            StatusNameAlreadyExistsException => StatusCodes.Status409Conflict,
            StatusHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
