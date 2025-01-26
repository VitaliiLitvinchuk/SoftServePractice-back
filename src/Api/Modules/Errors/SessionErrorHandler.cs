using Application.Sessions.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class SessionErrorHandler
{
    public static ObjectResult ToObjectResult(this SessionException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            SessionUnknownException => StatusCodes.Status400BadRequest,
            SessionNotFoundException => StatusCodes.Status404NotFound,
            StatusForSessionNotFoundException => StatusCodes.Status404NotFound,
            MovieForSessionNotFoundException => StatusCodes.Status404NotFound,
            HallForSessionNotFoundException => StatusCodes.Status404NotFound,
            SessionCannotBeShorterThanMovieDurationException => StatusCodes.Status409Conflict,
            HallWillHaveSessionInThisTimeException => StatusCodes.Status409Conflict,
            SessionHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
