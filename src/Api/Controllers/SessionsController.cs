using Api.Attributes;
using Api.Dtos.Sessions;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Sessions.Commands;
using Application.Sessions.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Movies;
using Domain.Sessions;
using Domain.Statuses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController(IBaseQuery<Session> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SessionDto>>> GetAll(CancellationToken cancellation)
        {
            var seats = await query.GetMany(cancellation, include: x => x.Include(x => x.Hall).Include(x => x.Status).Include(x => x.Movie).Include(x => x.Tickets)!);

            return Ok(seats.Select(SessionDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SessionDto>>> GetByHallId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var hallId = new HallId(id);

            var seats = await query.GetMany(cancellation, x => x.HallId == hallId);

            return Ok(seats.Select(SessionDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SessionDto>>> GetByStatusId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var statusId = new StatusId(id);

            var seats = await query.GetMany(cancellation, x => x.StatusId == statusId, include: x => x.Include(x => x.Hall).Include(x => x.Status).Include(x => x.Movie).Include(x => x.Tickets)!);

            return Ok(seats.Select(SessionDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<SessionDto>>> GetByMovieId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);

            var seats = await query.GetMany(cancellation, x => x.MovieId == movieId, include: x => x.Include(x => x.Hall).Include(x => x.Status).Include(x => x.Movie).Include(x => x.Tickets)!);

            return Ok(seats.Select(SessionDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<SessionDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var statusId = new SessionId(id);

            var result = await query.Get(cancellation, x => x.Id == statusId, include: x => x.Include(x => x.Hall).Include(x => x.Status).Include(x => x.Movie).Include(x => x.Tickets)!);

            return result.Match(
                session => Ok(SessionDto.FromDomainModel(session)),
                () => new SessionNotFoundException(statusId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<SessionDto>> Create([FromForm] CreateSessionDto dto, CancellationToken cancellation)
        {
            var input = new CreateSessionCommand
            {
                MovieId = dto.MovieId,
                HallId = dto.HallId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                session => Ok(SessionDto.FromDomainModel(session)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteSessionDto dto, CancellationToken cancellation)
        {
            var input = new DeleteSessionCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                seat => Ok(SessionDto.FromDomainModel(seat)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<SessionDto>> Update([FromForm] UpdateSessionDto dto, CancellationToken cancellation)
        {
            var input = new UpdateSessionCommand
            {
                Id = dto.Id,
                MovieId = dto.MovieId,
                HallId = dto.HallId,
                StatusId = dto.StatusId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                session => Ok(SessionDto.FromDomainModel(session)),
                e => e.ToObjectResult()
            );
        }
    }
}
