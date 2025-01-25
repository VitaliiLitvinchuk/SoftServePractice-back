using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Movies.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Movies.Commands;

public class DeleteMovieCommand : IRequest<Result<Movie, MovieException>>
{
    public required Guid Id { get; init; }
}

public class DeleteMovieCommandHandler(IBaseRepository<Movie> repository, IBaseQuery<Movie> query) : IRequestHandler<DeleteMovieCommand, Result<Movie, MovieException>>
{
    public async Task<Result<Movie, MovieException>> Handle(DeleteMovieCommand request, CancellationToken cancellation)
    {
        var id = new MovieId(request.Id);
        var result = await query.Get(cancellation, x => x.Id == id, include: x => x
            .Include(x => x.Genres)
            .Include(x => x.Tags)
            .Include(x => x.Sessions)
            .Include(x => x.Actors)
            .Include(x => x.Ratings));

        return await result.Match(
            entity => DeleteEntity(entity, cancellation),
            () => Task.FromResult<Result<Movie, MovieException>>(new MovieNotFoundException(id))
        );
    }

    private async Task<Result<Movie, MovieException>> DeleteEntity(Movie entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieUnknownException(entity.Id, exception);
        }
    }
}
