using Api.Attributes;
using Api.Dtos.GenresTags;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.GenresTags.Commands;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.GenresTags;
using Domain.Tags;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenresTagsController(IBaseQuery<GenreTag> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GenreTagDto>>> GetAll(CancellationToken cancellation)
        {
            var genrestags = await query.GetMany(cancellation, include: x => x.Include(x => x.Genre).Include(x => x.Tag)!);

            return Ok(genrestags.Select(GenreTagDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GenreTagDto>> GetByGenreId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var genreId = new GenreId(id);
            var genrestags = await query.GetMany(cancellation, x => x.GenreId == genreId, include: x => x.Include(x => x.Genre).Include(x => x.Tag)!);

            return Ok(genrestags.Select(GenreTagDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GenreTagDto>> GetByTagId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var tagId = new TagId(id);
            var genrestags = await query.GetMany(cancellation, x => x.TagId == tagId, include: x => x.Include(x => x.Genre).Include(x => x.Tag)!);

            return Ok(genrestags.Select(GenreTagDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<GenreTagDto>> Create([FromForm] CreateGenreTagDto dto, CancellationToken cancellation)
        {
            var input = new CreateGenreTagCommand
            {
                GenreId = dto.GenreId,
                TagId = dto.TagId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                genreTag => Ok(GenreTagDto.FromDomainModel(genreTag)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteGenreTagDto dto, CancellationToken cancellation)
        {
            var input = new DeleteGenreTagCommand
            {
                GenreId = dto.GenreId,
                TagId = dto.TagId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                genreTag => Ok(GenreTagDto.FromDomainModel(genreTag)),
                e => e.ToObjectResult()
            );
        }
    }
}
