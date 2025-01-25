using Api.Attributes;
using Api.Dtos.Halls;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Halls.Commands;
using Application.Halls.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HallsController(IBaseQuery<Hall> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<HallDto>>> GetAll(CancellationToken cancellation)
        {
            var halls = await query.GetMany(cancellation, include: x => x.Include(x => x.Seats).Include(x => x.Sessions));

            return Ok(halls.Select(HallDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<HallDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var hallId = new HallId(id);

            var result = await query.Get(cancellation, x => x.Id == hallId, include: x => x.Include(x => x.Seats).Include(x => x.Sessions));

            return result.Match(
                hall => Ok(HallDto.FromDomainModel(hall)),
                () => new HallNotFoundException(hallId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<HallDto>> Create([FromForm] CreateHallDto dto, CancellationToken cancellation)
        {
            var input = new CreateHallCommand
            {
                Name = dto.Name,
                Capacity = dto.Capacity
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                hall => Ok(HallDto.FromDomainModel(hall)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<HallDto>> Update([FromForm] UpdateHallDto dto, CancellationToken cancellation)
        {
            var input = new UpdateHallCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Capacity = dto.Capacity
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                hall => Ok(HallDto.FromDomainModel(hall)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteHallDto dto, CancellationToken cancellation)
        {
            var input = new DeleteHallCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                hall => Ok(HallDto.FromDomainModel(hall)),
                e => e.ToObjectResult()
            );
        }
    }
}
