using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using MediatR;

namespace Application.Movies.Commands;

public class UpdateMovieCommand : IRequest<Result<Movie, MovieException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required long Duration { get; init; }
    public required string TrailerUrl { get; init; }
    public required string ImageUrl { get; init; }
    public required string Description { get; init; }
    public required DateTime ReleaseDate { get; init; }
}

public class UpdateMovieCommandHandler(IBaseRepository<Movie> repository, IBaseQuery<Movie> query) : IRequestHandler<UpdateMovieCommand, Result<Movie, MovieException>>
{
    public async Task<Result<Movie, MovieException>> Handle(UpdateMovieCommand request, CancellationToken cancellation)
    {
        var id = new MovieId(request.Id);
        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity => await UpdateEntity(entity, request.Name, request.Duration, request.TrailerUrl, request.ImageUrl, request.Description, request.ReleaseDate, cancellation),
            () => Task.FromResult<Result<Movie, MovieException>>(new MovieNotFoundException(id))
        );
    }

    private async Task<Result<Movie, MovieException>> UpdateEntity(Movie entity, string name, long duration, string trailerUrl, string imageUrl, string description, DateTime releaseDate, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name, duration, trailerUrl, imageUrl, description, releaseDate);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieUnknownException(entity.Id, exception);
        }
    }
}
