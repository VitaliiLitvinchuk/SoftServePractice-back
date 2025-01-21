using Api.Dtos.Actors;
using Api.Modules.Errors;
using Application.Actors.Commands;
using Application.Actors.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using CSharpFunctionalExtensions;
using Domain.Actors;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActorsController(IBaseQuery<Actor> query, ISender sender, IFileService fileService) : ControllerBase
    {
        private static readonly string[] subFolders = ["images", "actors"];
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<ActorDto>>> GetAll(CancellationToken cancellation)
        {
            var actors = await query.GetMany(cancellation, include: x => x.Include(x => x.Movies).ThenInclude(x => x.Movie)!);

            return Ok(actors.Select(ActorDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ActorDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var actorId = new ActorId(id);

            var result = await query.Get(cancellation, x => x.Id == actorId, include: x => x.Include(x => x.Movies));

            return result.Match(
                actor => Ok(ActorDto.FromDomainModel(actor)),
                () => new ActorNotFoundException(actorId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ActorDto>> Create([FromForm] CreateActorDto dto, CancellationToken cancellation)
        {
            var fileUrl = await dto.Image.Save(subFolders, fileService, cancellation);

            var input = new CreateActorCommand
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Middlename = dto.Middlename,
                ImageUrl = fileUrl
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                actor => Ok(ActorDto.FromDomainModel(actor)),
                e =>
                {
                    fileService.DeleteFile(fileUrl);

                    return e.ToObjectResult();
                }
            );
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<ActorDto>> Update([FromForm] UpdateActorDto dto, CancellationToken cancellation)
        {
            var actor = await query.Get(filter: x => x.Id == new ActorId(dto.Id), cancellation: cancellation);

            var fileUrl = actor.Match(actor => actor.ImageUrl, () => string.Empty);
            var oldFileUrl = fileUrl;

            if (fileUrl == string.Empty)
                return new ActorNotFoundException(new ActorId(dto.Id)).ToObjectResult();

            bool fileChanged = false;
            if (dto.Image != null)
            {
                fileChanged = true;
                fileUrl = await dto.Image.Save(subFolders, fileService, cancellation);
            }

            var input = new UpdateActorCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Surname = dto.Surname,
                Middlename = dto.Middlename,
                ImageUrl = fileUrl
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                actor =>
                {
                    if (fileChanged)
                        fileService.DeleteFile(oldFileUrl);

                    return Ok(ActorDto.FromDomainModel(actor));
                },
                e =>
                {
                    if (fileChanged)
                        fileService.DeleteFile(fileUrl);

                    return e.ToObjectResult();
                }
            );
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult<ActorDto>> Delete([FromQuery] DeleteActorDto dto, CancellationToken cancellation)
        {
            var input = new DeleteActorCommand
            {
                Id = dto.Id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                actor =>
                {
                    fileService.DeleteFile(actor.ImageUrl);

                    return Ok(ActorDto.FromDomainModel(actor));
                },
                e => e.ToObjectResult()
            );
        }
    }
}