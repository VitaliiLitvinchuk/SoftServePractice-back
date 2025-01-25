using Api.Attributes;
using Api.Dtos.Genres;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Genres.Commands;
using Application.Genres.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenresController(IBaseQuery<Genre> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetAll(CancellationToken cancellation)
        {
            var genres = await query.GetMany(cancellation, include: x => x.Include(x => x.Movies).ThenInclude(x => x.Movie).Include(x => x.Tags).ThenInclude(x => x.Tag)!);

            return Ok(genres.Select(GenreDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<GenreDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var genreId = new GenreId(id);

            var result = await query.Get(cancellation, x => x.Id == genreId, include: x => x.Include(x => x.Movies).Include(x => x.Tags));

            return result.Match(
                genre => Ok(GenreDto.FromDomainModel(genre)),
                () => new GenreNotFoundException(genreId).ToObjectResult()
            );
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetBySimilarity([FromQuery] string name, CancellationToken cancellation)
        {
            const int maxReplacement = 3;
            var normalizedName = name.ToLowerInvariant().Trim();

            var genres = await query.GetMany(cancellation,
                x => EF.Functions.ILike(x.Name.Trim(), $"%{normalizedName}%")
                || PgFunctions.PgSimilarity(x.Name.Trim(), normalizedName) > 0.3
                || PgFunctions.PgFuzzyMatch(x.Name.Trim(), normalizedName) <= maxReplacement,
                include: x => x.Include(x => x.Movies).Include(x => x.Tags));

            return Ok(genres.Select(GenreDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<GenreDto>> Create([FromForm] CreateGenreDto dto, CancellationToken cancellation)
        {
            var input = new CreateGenreCommand
            {
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                genre => Ok(GenreDto.FromDomainModel(genre)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<GenreDto>> Update([FromForm] UpdateGenreDto dto, CancellationToken cancellation)
        {
            var input = new UpdateGenreCommand
            {
                Id = dto.Id,
                Name = dto.Name
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                genre => Ok(GenreDto.FromDomainModel(genre)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteGenreDto dto, CancellationToken cancellation)
        {
            var input = new DeleteGenreCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                genre => Ok(GenreDto.FromDomainModel(genre)),
                e => e.ToObjectResult()
            );
        }
    }
}
