using Api.Attributes;
using Api.Dtos.MoviesRating;
using Api.Modules.Errors;
using Application;
using Application.Common.Interfaces.Queries;
using Application.MoviesRatings.Commands;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesRatings;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MoviesRatingsController(IBaseQuery<MovieRating> query, ISender sender) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<MovieRatingDto>>> GetAll(CancellationToken cancellation)
        {
            var moviesRatings = await query.GetMany(cancellation, include: x => x.Include(x => x.Movie).Include(x => x.User)!);

            return Ok(moviesRatings.Select(MovieRatingDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieRatingDto>> GetByMovieId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var movieId = new MovieId(id);
            var moviesRatings = await query.GetMany(cancellation, x => x.MovieId == movieId, include: x => x.Include(x => x.Movie).Include(x => x.User)!);

            return Ok(moviesRatings.Select(MovieRatingDto.FromDomainModel));
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<MovieRatingDto>> GetByUserId([FromQuery] Guid id, CancellationToken cancellation)
        {
            var userId = new UserId(id);
            var moviesRatings = await query.GetMany(cancellation, x => x.UserId == userId, include: x => x.Include(x => x.Movie).Include(x => x.User)!);

            return Ok(moviesRatings.Select(MovieRatingDto.FromDomainModel));
        }

        [HttpPost("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieRatingDto>> Create([FromForm] CreateMovieRatingDto dto, CancellationToken cancellation)
        {
            var input = new CreateMovieRatingCommand
            {
                MovieId = dto.MovieId,
                UserId = dto.UserId,
                Rating = dto.Rating
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieRating => Ok(MovieRatingDto.FromDomainModel(movieRating)),
                e => e.ToObjectResult()
            );
        }

        [HttpPut("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult<MovieRatingDto>> Update([FromForm] UpdateMovieRatingDto dto, CancellationToken cancellation)
        {
            var input = new UpdateMovieRatingCommand
            {
                MovieId = dto.MovieId,
                UserId = dto.UserId,
                Rating = dto.Rating
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieRating => Ok(MovieRatingDto.FromDomainModel(movieRating)),
                e => e.ToObjectResult()
            );
        }

        [HttpDelete("[action]")]
        [TypeFilter(typeof(Authorized), Arguments = [Defaults.AdminRole])]
        public async Task<ActionResult> Delete([FromQuery] DeleteMovieRatingDto dto, CancellationToken cancellation)
        {
            var input = new DeleteMovieRatingCommand
            {
                MovieId = dto.MovieId,
                UserId = dto.UserId
            };

            var result = await sender.Send(input, cancellation);

            return result.Match(
                movieRating => Ok(MovieRatingDto.FromDomainModel(movieRating)),
                e => e.ToObjectResult()
            );
        }
    }
}
