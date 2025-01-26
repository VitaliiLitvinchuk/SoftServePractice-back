using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesTags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesTags;
using Domain.Tags;
using MediatR;

namespace Application.MoviesTags.Commands;

public class DeleteMovieTagCommand : IRequest<Result<MovieTag, MovieTagException>>
{
    public required Guid MovieId { get; init; }
    public required Guid TagId { get; init; }
}

public class DeleteMovieTagCommandHandler(IBaseRepository<MovieTag> repository, IBaseQuery<MovieTag> query) : IRequestHandler<DeleteMovieTagCommand, Result<MovieTag, MovieTagException>>
{
    public async Task<Result<MovieTag, MovieTagException>> Handle(DeleteMovieTagCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var tagId = new TagId(request.TagId);

        var entity = MovieTag.New(movieId, tagId);

        var result = await query.Get(cancellation, x => x.MovieId == entity.MovieId && x.TagId == entity.TagId);

        return await result.Match(
            async entity => await DeleteEntity(entity, cancellation),
            () => Task.FromResult<Result<MovieTag, MovieTagException>>(new MovieTagNotFoundException(entity.MovieId, entity.TagId))
        );
    }

    private async Task<Result<MovieTag, MovieTagException>> DeleteEntity(MovieTag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieTagUnknownException(entity.MovieId, entity.TagId, exception);
        }
    }
}
