using System.IdentityModel.Tokens.Jwt;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Domain.Roles;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Optional.Unsafe;

namespace Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class Authorized(IJwtService jwtService, IBaseQuery<User> users, string? allowedRole = null) : AuthorizeAttribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").LastOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!await jwtService.ValidateToken(token, default))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValues = tokenHandler.ReadJwtToken(token);

            var userId = new UserId(Guid.Parse(tokenValues.Claims.First(c => c.Type == "userId").Value));

            var result = await users.Get(default, x => x.Id == userId, include: x => x.Include(x => x.Role)!);
            var user = result.ValueOrDefault();

            if (result == default)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleId = new RoleId(Guid.Parse(tokenValues.Claims.First(c => c.Type == "roleId").Value));
            if (allowedRole != null)
            {
                if (user.RoleId != roleId || user.Role!.Name != allowedRole)
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
            await next();
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}