using Api.Attributes;
using Api.Dtos.Seats;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Seats.Commands;
using Application.Seats.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Seats;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SeatsController(IBaseQuery<Seat> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetAll(CancellationToken cancellation)
        {
            var seats = await query.GetMany(cancellation, include: x => x.Include(x => x.Hall)!);

            return Ok(seats.Select(SeatDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetByHallId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var hallId = new HallId(id);

            var seats = await query.GetMany(cancellation, x => x.HallId == hallId);

            return Ok(seats.Select(SeatDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<SeatDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var statusId = new SeatId(id);

            var result = await query.Get(cancellation, x => x.Id == statusId, include: x => x.Include(x => x.Hall)!);

            return result.Match(
                seat => Ok(SeatDto.FromDomainModel(seat)),
                () => new SeatNotFoundException(statusId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<SeatDto>> Create([FromForm] CreateSeatDto dto, CancellationToken cancellation)
        {
            var input = new CreateSeatCommand
            {
                Row = dto.Row,
                Number = dto.Number,
                HallId = dto.HallId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                seat => Ok(SeatDto.FromDomainModel(seat)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<SeatDto>> Update([FromForm] UpdateSeatDto dto, CancellationToken cancellation)
        {
            var input = new UpdateSeatCommand
            {
                Id = dto.Id,
                Row = dto.Row,
                Number = dto.Number,
                HallId = dto.HallId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                seat => Ok(SeatDto.FromDomainModel(seat)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteSeatDto dto, CancellationToken cancellation)
        {
            var input = new DeleteSeatCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                seat => Ok(SeatDto.FromDomainModel(seat)),
                e => e.ToObjectResult()
            );
        }
    }
}
