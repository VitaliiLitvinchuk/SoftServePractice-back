using Application.MoviesTags.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieTagErrorHandler
{
    public static ObjectResult ToObjectResult(this MovieTagException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            MovieTagUnknownException => StatusCodes.Status400BadRequest,
            MovieTagNotFoundException => StatusCodes.Status404NotFound,
            MovieForMovieTagNotFoundException => StatusCodes.Status404NotFound,
            TagForMovieTagNotFoundException => StatusCodes.Status404NotFound,
            MovieTagAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
