using Application.MoviesActors.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieActorErrorHandler
{
    public static ObjectResult ToObjectResult(this MovieActorException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            MovieActorUnknownException => StatusCodes.Status400BadRequest,
            MovieActorNotFoundException => StatusCodes.Status404NotFound,
            MovieForMovieActorNotFoundException => StatusCodes.Status404NotFound,
            ActorForMovieActorNotFoundException => StatusCodes.Status404NotFound,
            MovieActorAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
