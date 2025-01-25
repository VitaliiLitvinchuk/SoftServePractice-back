using Api.Attributes;
using Api.Dtos.MoviesActors;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.MoviesActors.Commands;
using CSharpFunctionalExtensions;
using Domain.Actors;
using Domain.Movies;
using Domain.MoviesActors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesActorsController(IBaseQuery<MovieActor> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MovieActorDto>>> GetAll(CancellationToken cancellation)
        {
            var moviesActors = await query.GetMany(cancellation, include: x => x.Include(x => x.Movie).Include(x => x.Actor)!);

            return Ok(moviesActors.Select(MovieActorDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieActorDto>> GetByMovieId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);
            var moviesActors = await query.GetMany(cancellation, x => x.MovieId == movieId, include: x => x.Include(x => x.Movie).Include(x => x.Actor)!);

            return Ok(moviesActors.Select(MovieActorDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieActorDto>> GetByActorId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var actorId = new ActorId(id);
            var moviesActors = await query.GetMany(cancellation, x => x.ActorId == actorId, include: x => x.Include(x => x.Movie).Include(x => x.Actor)!);

            return Ok(moviesActors.Select(MovieActorDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieActorDto>> Create([FromForm] CreateMovieActorDto dto, CancellationToken cancellation)
        {
            var input = new CreateMovieActorCommand
            {
                MovieId = dto.MovieId,
                ActorId = dto.ActorId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieActor => Ok(MovieActorDto.FromDomainModel(movieActor)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteMovieActorDto dto, CancellationToken cancellation)
        {
            var input = new DeleteMovieActorCommand
            {
                MovieId = dto.MovieId,
                ActorId = dto.ActorId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieActor => Ok(MovieActorDto.FromDomainModel(movieActor)),
                e => e.ToObjectResult()
            );
        }
    }
}
