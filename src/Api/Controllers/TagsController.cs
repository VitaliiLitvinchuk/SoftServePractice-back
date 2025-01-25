using Api.Attributes;
using Api.Dtos.Tags;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Tags.Commands;
using Application.Tags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tags;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TagsController(IBaseQuery<Tag> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetAll(CancellationToken cancellation)
        {
            var tags = await query.GetMany(cancellation, include: x => x.Include(x => x.Movies).ThenInclude(x => x.Movie).Include(x => x.Genres).ThenInclude(x => x.Genre)!);

            return Ok(tags.Select(TagDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<TagDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var tagId = new TagId(id);

            var result = await query.Get(cancellation, x => x.Id == tagId, include: x => x.Include(x => x.Movies).Include(x => x.Genres));

            return result.Match(
                tag => Ok(TagDto.FromDomainModel(tag)),
                () => new TagNotFoundException(tagId).ToObjectResult()
            );
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetBySimilarity([FromQuery] string name, CancellationToken cancellation)
        {
            const int maxReplacement = 3;
            var normalizedName = name.ToLowerInvariant().Trim();

            var tags = await query.GetMany(cancellation,
                x => EF.Functions.ILike(x.Name.Trim(), $"%{normalizedName}%")
                || PgFunctions.PgSimilarity(x.Name.Trim(), normalizedName) > 0.3
                || PgFunctions.PgFuzzyMatch(x.Name.Trim(), normalizedName) <= maxReplacement,
                include: x => x.Include(x => x.Movies).Include(x => x.Genres));

            return Ok(tags.Select(TagDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<TagDto>> Create([FromForm] CreateTagDto dto, CancellationToken cancellation)
        {
            var input = new CreateTagCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                tag => Ok(TagDto.FromDomainModel(tag)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<TagDto>> Update([FromForm] UpdateTagDto dto, CancellationToken cancellation)
        {
            var input = new UpdateTagCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                tag => Ok(TagDto.FromDomainModel(tag)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteTagDto dto, CancellationToken cancellation)
        {
            var input = new DeleteTagCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                tag => Ok(TagDto.FromDomainModel(tag)),
                e => e.ToObjectResult()
            );
        }
    }
}
