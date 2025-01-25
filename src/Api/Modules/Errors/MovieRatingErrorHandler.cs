using Application.MoviesRatings.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieRatingErrorHandler
{
    public static ObjectResult ToObjectResult(this MovieRatingException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            MovieRatingUnknownException => StatusCodes.Status400BadRequest,
            MovieRatingNotFoundException => StatusCodes.Status404NotFound,
            MovieForMovieRatingNotFoundException => StatusCodes.Status404NotFound,
            UserForMovieRatingNotFoundException => StatusCodes.Status404NotFound,
            MovieRatingAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
