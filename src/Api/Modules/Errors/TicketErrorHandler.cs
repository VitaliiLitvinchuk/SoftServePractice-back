using Application.Tickets.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class TicketErrorHandler
{
    public static ObjectResult ToObjectResult(this TicketException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            TicketUnknownException => StatusCodes.Status400BadRequest,
            SessionForTicketNotFoundException => StatusCodes.Status404NotFound,
            SeatForTicketNotFoundException => StatusCodes.Status404NotFound,
            TicketNotFoundException => StatusCodes.Status404NotFound,
            TicketHasReleationsException => StatusCodes.Status409Conflict,
            TicketAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
