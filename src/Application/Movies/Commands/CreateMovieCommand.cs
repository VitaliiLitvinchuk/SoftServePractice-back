using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using MediatR;

namespace Application.Movies.Commands;

public class CreateMovieCommand : IRequest<Result<Movie, MovieException>>
{
    public required string Name { get; init; }
    public required long Duration { get; init; }
    public required string TrailerUrl { get; init; }
    public required string ImageUrl { get; init; }
    public required string Description { get; init; }
    public required DateTime ReleaseDate { get; init; }
}

public class CreateMovieCommandHandler(IBaseRepository<Movie> repository) : IRequestHandler<CreateMovieCommand, Result<Movie, MovieException>>
{
    public async Task<Result<Movie, MovieException>> Handle(CreateMovieCommand request, CancellationToken cancellation)
    {
        var id = MovieId.New();
        var entity = Movie.New(id, request.Name, request.Duration, request.TrailerUrl, request.ImageUrl, request.Description, request.ReleaseDate);

        return await CreateEntity(entity, cancellation);
    }

    private async Task<Result<Movie, MovieException>> CreateEntity(Movie entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieUnknownException(entity.Id, exception);
        }
    }
}
