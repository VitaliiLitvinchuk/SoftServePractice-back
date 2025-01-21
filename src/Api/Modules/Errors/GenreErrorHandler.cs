using Application.Genres.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class GenreErrorHandler
{
    public static ObjectResult ToObjectResult(this GenreException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            GenreUnknownException => StatusCodes.Status400BadRequest,
            GenreNotFoundException => StatusCodes.Status404NotFound,
            GenreNameAlreadyExistsException => StatusCodes.Status409Conflict,
            GenreHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
