using Api.Attributes;
using Api.Dtos.Movies;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Application.Movies.Commands;
using Application.Movies.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesController(IBaseQuery<Movie> query, ISender sender, IFileService fileService) : ControllerBase
    {
        private static readonly string[] subFolders = ["images", "movies"];
        // TODD: then include
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetAll(CancellationToken cancellation)
        {
            var movies = await query.GetMany(cancellation, include: x => x
                .Include(x => x.Genres)
                .Include(x => x.Tags)
                .Include(x => x.Sessions)
                .Include(x => x.Actors)
                .Include(x => x.Ratings));

            return Ok(movies.Select(MovieDto.FromDomainModel));
        }

        // TODD: then include
        [HttpGet("[action]")]
        public async Task<ActionResult<MovieDto>> GetById([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);

            var movie = await query.Get(cancellation, x => x.Id == movieId, include: x => x
                .Include(x => x.Genres)
                .Include(x => x.Tags)
                .Include(x => x.Sessions)
                .Include(x => x.Actors)
                .Include(x => x.Ratings));

            return movie.Match(
                movie => Ok(MovieDto.FromDomainModel(movie)),
                () => new MovieNotFoundException(movieId).ToObjectResult()
            );
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieDto>> Create([FromForm] CreateMovieDto dto, CancellationToken cancellation)
        {
            var fileUrl = await dto.Image.Save(subFolders, fileService, cancellation);

            var input = new CreateMovieCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                ReleaseDate = dto.ReleaseDate,
                Duration = dto.Duration,
                TrailerUrl = dto.TrailerUrl,
                ImageUrl = fileUrl,
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movie => Ok(MovieDto.FromDomainModel(movie)),
                e =>
                {
                    fileService.DeleteFile(fileUrl);

                    return e.ToObjectResult();
                }
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieDto>> Update([FromForm] UpdateMovieDto dto, CancellationToken cancellation)
        {
            var actor = await query.Get(filter: x => x.Id == new MovieId(dto.Id), cancellation: cancellation);

            var fileUrl = actor.Match(actor => actor.ImageUrl, () => string.Empty);
            var oldFileUrl = fileUrl;

            if (fileUrl == string.Empty)
                return new MovieNotFoundException(new MovieId(dto.Id)).ToObjectResult();

            bool fileChanged = false;
            if (dto.Image != null)
            {
                fileChanged = true;
                fileUrl = await dto.Image.Save(subFolders, fileService, cancellation);
            }

            var input = new UpdateMovieCommand
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                ReleaseDate = dto.ReleaseDate,
                Duration = dto.Duration,
                TrailerUrl = dto.TrailerUrl,
                ImageUrl = fileUrl,
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movie =>
                {
                    if (fileChanged)
                        fileService.DeleteFile(oldFileUrl);

                    return Ok(MovieDto.FromDomainModel(movie));
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
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] Guid id, CancellationToken cancellation)
        {
            var input = new DeleteMovieCommand
            {
                Id = id
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movie =>
                {
                    fileService.DeleteFile(movie.ImageUrl);

                    return Ok(MovieDto.FromDomainModel(movie));
                },
                e => e.ToObjectResult()
            );
        }
    }
}
