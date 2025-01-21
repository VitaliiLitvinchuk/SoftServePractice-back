using System;
using Api.Attributes;
using Api.Dtos.Users;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Application.Users.Commands;
using Application.Users.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController(IBaseQuery<User> query, IJwtService jwtService, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(CancellationToken cancellation)
        {
            var users = await query.GetMany(cancellation, include: x => x.Include(x => x.Role)!);

            return Ok(users.Select(UserDto.FromDomainModel));
        }

        [TypeFilter(typeof(Authorized))]
        [HttpGet("[action]")]
        public async Task<ActionResult<UserDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var userId = new UserId(id);

            var result = await query.Get(cancellation, x => x.Id == userId, include: x => x.Include(x => x.Role)!);

            return result.Match(
                user => Ok(UserDto.FromDomainModel(user)),
                () => new UserNotFoundException(userId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> Login([FromForm] LoginDto dto, CancellationToken cancellation)
        {
            var command = new LoginCommand
            {
                Email = dto.Email,
                Password = dto.Password
            };

            var result = await sender.Send(command, cancellation);

            return result.Match(
                user => Ok(new { token = jwtService.GenerateToken(user, cancellation).Result }),
                e => e.ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> Create([FromBody] CreateUserDto dto, CancellationToken cancellation)
        {
            var command = new CreateUserCommand
            {
                Email = dto.Email,
                Password = dto.Password
            };

            var result = await sender.Send(command, cancellation);

            return result.Match(
                user => Ok(new { token = jwtService.GenerateToken(user, cancellation).Result }),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized))]
        public async Task<ActionResult<string>> Update([FromForm] UpdateUserDto dto, CancellationToken cancellation)
        {
            var input = new UpdateUserCommand
            {
                Id = dto.Id,
                RoleId = dto.RoleId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                user => Ok(new { token = jwtService.GenerateToken(user, cancellation).Result }),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized))]
        public async Task<ActionResult<UserDto>> Delete([FromQuery] Guid id, CancellationToken cancellation)
        {
            var input = new DeleteUserCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                user => Ok(UserDto.FromDomainModel(user)),
                e => e.ToObjectResult()
            );
        }
    }
}