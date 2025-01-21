using Application.Roles.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class RoleErrorHandler
{
    public static ObjectResult ToObjectResult(this RoleException exception) => new(new { errors = new { server = exception.Message } })
    {
        StatusCode = exception switch
        {
            RoleUnknownException => StatusCodes.Status400BadRequest,
            RoleNotFoundException => StatusCodes.Status404NotFound,
            RoleNameAlreadyExistsException => StatusCodes.Status409Conflict,
            RoleHasReleationsException => StatusCodes.Status409Conflict,
            _ => throw new NotImplementedException("Unhandled exception")
        }
    };
}
