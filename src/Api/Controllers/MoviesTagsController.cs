using Api.Attributes;
using Api.Dtos.MoviesTags;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.MoviesTags.Commands;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesTags;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesTagsController(IBaseQuery<MovieTag> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MovieTagDto>>> GetAll(CancellationToken cancellation)
        {
            var moviesTags = await query.GetMany(cancellation, include: x => x.Include(x => x.Movie).Include(x => x.Tag)!);

            return Ok(moviesTags.Select(MovieTagDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieTagDto>> GetByMovieId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);
            var moviesTags = await query.GetMany(cancellation, x => x.MovieId == movieId, include: x => x.Include(x => x.Movie).Include(x => x.Tag)!);

            return Ok(moviesTags.Select(MovieTagDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieTagDto>> GetByTagId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var tagId = new TagId(id);
            var moviesTags = await query.GetMany(cancellation, x => x.TagId == tagId, include: x => x.Include(x => x.Movie).Include(x => x.Tag)!);

            return Ok(moviesTags.Select(MovieTagDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieTagDto>> Create([FromForm] CreateMovieTagDto dto, CancellationToken cancellation)
        {
            var input = new CreateMovieTagCommand
            {
                MovieId = dto.MovieId,
                TagId = dto.TagId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieTag => Ok(MovieTagDto.FromDomainModel(movieTag)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteMovieTagDto dto, CancellationToken cancellation)
        {
            var input = new DeleteMovieTagCommand
            {
                MovieId = dto.MovieId,
                TagId = dto.TagId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieTag => Ok(MovieTagDto.FromDomainModel(movieTag)),
                e => e.ToObjectResult()
            );
        }
    }
}
