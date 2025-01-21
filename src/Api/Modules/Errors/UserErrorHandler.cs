using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            UserUnknownException => StatusCodes.Status400BadRequest,
            UserInvalidDataException => StatusCodes.Status400BadRequest,
            RoleForUserNotFoundException => StatusCodes.Status400BadRequest,
            UserNotFoundException => StatusCodes.Status404NotFound,
            UserEmailAlreadyExistsException => StatusCodes.Status409Conflict,
            UserHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
