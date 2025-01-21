using Api.Dtos.Roles;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Roles.Commands;
using Application.Roles.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController(IBaseQuery<Role> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll(CancellationToken cancellation)
        {
            var roles = await query.GetMany(cancellation, include: x => x.Include(x => x.Users));

            return Ok(roles.Select(RoleDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<RoleDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var roleId = new RoleId(id);

            var result = await query.Get(cancellation, x => x.Id == roleId, include: x => x.Include(x => x.Users));

            return result.Match(
                role => Ok(RoleDto.FromDomainModel(role)),
                () => new RoleNotFoundException(roleId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<RoleDto>> Create([FromForm] CreateRoleDto dto, CancellationToken cancellation)
        {
            var input = new CreateRoleCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                role => Ok(RoleDto.FromDomainModel(role)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<RoleDto>> Update([FromForm] UpdateRoleDto dto, CancellationToken cancellation)
        {
            var input = new UpdateRoleCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                role => Ok(RoleDto.FromDomainModel(role)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete([FromQuery] DeleteRoleDto dto, CancellationToken cancellation)
        {
            var input = new DeleteRoleCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                role => Ok(RoleDto.FromDomainModel(role)),
                e => e.ToObjectResult()
            );
        }
    }
}
