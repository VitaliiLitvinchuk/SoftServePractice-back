using Application.MoviesGenres.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class MovieGenreErrorHandler
{
    public static ObjectResult ToObjectResult(this MovieGenreException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            MovieGenreUnknownException => StatusCodes.Status400BadRequest,
            MovieGenreNotFoundException => StatusCodes.Status404NotFound,
            MovieForMovieGenreNotFoundException => StatusCodes.Status404NotFound,
            GenreForMovieGenreNotFoundException => StatusCodes.Status404NotFound,
            MovieGenreAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
