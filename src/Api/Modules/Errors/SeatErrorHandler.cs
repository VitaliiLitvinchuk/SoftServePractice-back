using Application.Seats.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class SeatErrorHandler
{
    public static ObjectResult ToObjectResult(this SeatException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            SeatUnknownException => StatusCodes.Status400BadRequest,
            SeatNotFoundException => StatusCodes.Status404NotFound,
            HallForSeatNotFoundException => StatusCodes.Status404NotFound,
            HallIsFullException => StatusCodes.Status409Conflict,
            SeatAlreadyExistsException => StatusCodes.Status409Conflict,
            SeatHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
