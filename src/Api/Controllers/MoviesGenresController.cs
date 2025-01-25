using Api.Attributes;
using Api.Dtos.MoviesGenres;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.MoviesGenres.Commands;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.Movies;
using Domain.MoviesGenres;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesGenresController(IBaseQuery<MovieGenre> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MovieGenreDto>>> GetAll(CancellationToken cancellation)
        {
            var moviesGenres = await query.GetMany(cancellation, include: x => x.Include(x => x.Movie).Include(x => x.Genre)!);

            return Ok(moviesGenres.Select(MovieGenreDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieGenreDto>> GetByMovieId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);
            var moviesGenres = await query.GetMany(cancellation, x => x.MovieId == movieId, include: x => x.Include(x => x.Movie).Include(x => x.Genre)!);

            return Ok(moviesGenres.Select(MovieGenreDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieGenreDto>> GetByGenreId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var genreId = new GenreId(id);
            var moviesGenres = await query.GetMany(cancellation, x => x.GenreId == genreId, include: x => x.Include(x => x.Movie).Include(x => x.Genre)!);

            return Ok(moviesGenres.Select(MovieGenreDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieGenreDto>> Create([FromForm] CreateMovieGenreDto dto, CancellationToken cancellation)
        {
            var input = new CreateMovieGenreCommand
            {
                MovieId = dto.MovieId,
                GenreId = dto.GenreId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieGenre => Ok(MovieGenreDto.FromDomainModel(movieGenre)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteMovieGenreDto dto, CancellationToken cancellation)
        {
            var input = new DeleteMovieGenreCommand
            {
                MovieId = dto.MovieId,
                GenreId = dto.GenreId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieGenre => Ok(MovieGenreDto.FromDomainModel(movieGenre)),
                e => e.ToObjectResult()
            );
        }
    }
}
