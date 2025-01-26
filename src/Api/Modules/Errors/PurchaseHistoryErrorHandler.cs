using Application.PurchaseHistories.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class PurchaseHistoryErrorHandler
{
    public static ObjectResult ToObjectResult(this PurchaseHistoryException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            PurchaseHistoryUnknownException => StatusCodes.Status400BadRequest,
            TicketForPurchaseHistoryNotFoundException => StatusCodes.Status404NotFound,
            UserForPurchaseHistoryNotFoundException => StatusCodes.Status404NotFound,
            PurchaseHistoryNotFoundException => StatusCodes.Status404NotFound,
            PurchaseHistoryAlreadyExistsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
