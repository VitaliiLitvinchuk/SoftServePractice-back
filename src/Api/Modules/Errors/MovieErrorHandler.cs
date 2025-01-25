using Application.Movies.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieErrorHandler
{
    public static ObjectResult ToObjectResult(this MovieException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            MovieUnknownException => StatusCodes.Status400BadRequest,
            MovieNotFoundException => StatusCodes.Status404NotFound,
            MovieHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
