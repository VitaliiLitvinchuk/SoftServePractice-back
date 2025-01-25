using Api.Attributes;
using Api.Dtos.Statuses;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Statuses.Commands;
using Application.Statuses.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Statuses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusesController(IBaseQuery<Status> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetAll(CancellationToken cancellation)
        {
            var statuses = await query.GetMany(cancellation, include: x => x.Include(x => x.Sessions));

            return Ok(statuses.Select(StatusDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<StatusDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var statusId = new StatusId(id);

            var result = await query.Get(cancellation, x => x.Id == statusId, include: x => x.Include(x => x.Sessions));

            return result.Match(
                statuse => Ok(StatusDto.FromDomainModel(statuse)),
                () => new StatusNotFoundException(statusId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<StatusDto>> Create([FromForm] CreateStatusDto dto, CancellationToken cancellation)
        {
            var input = new CreateStatusCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                statuse => Ok(StatusDto.FromDomainModel(statuse)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<StatusDto>> Update([FromForm] UpdateStatusDto dto, CancellationToken cancellation)
        {
            var input = new UpdateStatusCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                statuse => Ok(StatusDto.FromDomainModel(statuse)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteStatusDto dto, CancellationToken cancellation)
        {
            var input = new DeleteStatusCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                statuse => Ok(StatusDto.FromDomainModel(statuse)),
                e => e.ToObjectResult()
            );
        }
    }
}
