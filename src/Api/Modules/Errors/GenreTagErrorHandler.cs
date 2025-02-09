using Application.GenresTags.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class GenreTagErrorHandler
{
    public static ObjectResult ToObjectResult(this GenreTagException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            GenreTagUnknownException => StatusCodes.Status400BadRequest,
            GenreTagNotFoundException => StatusCodes.Status404NotFound,
            GenreForGenreTagNotFoundException => StatusCodes.Status404NotFound,
            TagForGenreTagNotFoundException => StatusCodes.Status404NotFound,
            GenreTagAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
