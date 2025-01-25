using Application.Halls.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class HallErrorHandler
{
    public static ObjectResult ToObjectResult(this HallException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            HallUnknownException => StatusCodes.Status400BadRequest,
            HallNotFoundException => StatusCodes.Status404NotFound,
            HallNameAlreadyExistsException => StatusCodes.Status409Conflict,
            HallHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
