using Api.Attributes;
using Api.Dtos.Tickets;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Tickets.Commands;
using Application.Tickets.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Seats;
using Domain.Sessions;
using Domain.Tickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TicketsController(IBaseQuery<Ticket> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAll(CancellationToken cancellation)
        {
            var tickets = await query.GetMany(cancellation, include: x => x.Include(x => x.Session).Include(x => x.Seat)!.Include(x => x.PurchaseHistories));

            return Ok(tickets.Select(TicketDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TicketDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var ticketId = new TicketId(id);

            var result = await query.Get(cancellation, x => x.Id == ticketId, include: x => x.Include(x => x.Session).Include(x => x.Seat)!.Include(x => x.PurchaseHistories));

            return result.Match(
                ticket => Ok(TicketDto.FromDomainModel(ticket)),
                () => new TicketNotFoundException(ticketId).ToObjectResult()
            );
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TicketDto>> GetBySessionId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var sessionId = new SessionId(id);

            var tickets = await query.GetMany(cancellation, x => x.SessionId == sessionId, include: x => x.Include(x => x.Session).Include(x => x.Seat)!.Include(x => x.PurchaseHistories));

            return Ok(tickets.Select(TicketDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TicketDto>> GetBySeatId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var seatId = new SeatId(id);

            var tickets = await query.GetMany(cancellation, x => x.SeatId == seatId, include: x => x.Include(x => x.Session).Include(x => x.Seat)!.Include(x => x.PurchaseHistories));

            return Ok(tickets.Select(TicketDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<TicketDto>> Create([FromForm] CreateTicketDto dto, CancellationToken cancellation)
        {
            var input = new CreateTicketCommand
            {
                SessionId = dto.SessionId,
                SeatId = dto.SeatId,
                Price = dto.Price
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                ticket => Ok(TicketDto.FromDomainModel(ticket)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<TicketDto>> Update([FromForm] UpdateTicketDto dto, CancellationToken cancellation)
        {
            var input = new UpdateTicketCommand
            {
                Id = dto.Id,
                Price = dto.Price
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                ticket => Ok(TicketDto.FromDomainModel(ticket)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<TicketDto>> Delete([FromQuery] Guid id, CancellationToken cancellation)
        {
            var input = new DeleteTicketCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                ticket => Ok(TicketDto.FromDomainModel(ticket)),
                e => e.ToObjectResult()
            );
        }
    }
}
