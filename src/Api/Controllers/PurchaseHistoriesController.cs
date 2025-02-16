using Api.Attributes;
using Api.Dtos.PurchaseHistories;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.PurchaseHistories.Commands;
using Application.PurchaseHistories.Exceptions;
using CSharpFunctionalExtensions;
using Domain.PurchaseHistories;
using Domain.Sessions;
using Domain.Tickets;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseHistoriesController(IBaseQuery<PurchaseHistory> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetAll(CancellationToken cancellation)
        {
            var histories = await query.GetMany(cancellation, include: x => x.Include(x => x.User).Include(x => x.Ticket)!);

            return Ok(histories.Select(PurchaseHistoryDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<PurchaseHistoryDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var historyId = new PurchaseHistoryId(id);
            var history = await query.Get(cancellation, x => x.Id == historyId);

            return history.Match(
                history => Ok(PurchaseHistoryDto.FromDomainModel(history)),
                () => new PurchaseHistoryNotFoundException(historyId).ToObjectResult()
            );
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetByUserId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var userId = new UserId(id);
            var histories = await query.GetMany(cancellation, x => x.UserId == userId);

            return Ok(histories.Select(PurchaseHistoryDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        [TypeFilter(typeof(Authorized))]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetByUser(CancellationToken cancellation)
        {
            var userId = (HttpContext.Items[Authorized.UserKey] as User)!.Id;
            var histories = await query.GetMany(cancellation, x => x.UserId == userId);

            return Ok(histories.Select(PurchaseHistoryDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetByTicketId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var ticketId = new TicketId(id);
            var histories = await query.GetMany(cancellation, x => x.TicketId == ticketId);

            return Ok(histories.Select(PurchaseHistoryDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetBySessionId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var sessionId = new SessionId(id);
            var histories = await query.GetMany(cancellation, x => x.Ticket != null && x.Ticket.SessionId == sessionId, include: x => x.Include(x => x.Ticket)!);

            return Ok(histories.Select(PurchaseHistoryDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<PurchaseHistoryDto>> Create([FromForm] CreatePurchaseHistoryDto dto, CancellationToken cancellation)
        {
            var input = new CreatePurchaseHistoryCommand
            {
                UserId = dto.UserId,
                TicketId = dto.TicketId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                history => Ok(PurchaseHistoryDto.FromDomainModel(history)),
                e => e.ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized))]
        public async Task<ActionResult<PurchaseHistoryDto>> CreateByUser([FromForm] Guid ticketId, CancellationToken cancellation)
        {
            var input = new CreatePurchaseHistoryCommand
            {
                UserId = (HttpContext.Items[Authorized.UserKey] as User)!.Id.Value,
                TicketId = ticketId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                history => Ok(PurchaseHistoryDto.FromDomainModel(history)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized))]
        public async Task<ActionResult<PurchaseHistoryDto>> Delete([FromQuery] Guid id, CancellationToken cancellation)
        {
            var input = new DeletePurchaseHistoryCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                history => Ok(PurchaseHistoryDto.FromDomainModel(history)),
                e => e.ToObjectResult()
            );
        }
    }
}
