using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.MoviesTags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Movies;
using Domain.MoviesTags;
using Domain.Tags;
using MediatR;

namespace Application.MoviesTags.Commands;

public class CreateMovieTagCommand : IRequest<Result<MovieTag, MovieTagException>>
{
    public required Guid MovieId { get; init; }
    public required Guid TagId { get; init; }
}

public class CreateMovieTagCommandHandler(IBaseRepository<MovieTag> repository, IBaseQuery<MovieTag> query) : IRequestHandler<CreateMovieTagCommand, Result<MovieTag, MovieTagException>>
{
    public async Task<Result<MovieTag, MovieTagException>> Handle(CreateMovieTagCommand request, CancellationToken cancellation)
    {
        var movieId = new MovieId(request.MovieId);
        var tagId = new TagId(request.TagId);

        var entity = MovieTag.New(movieId, tagId);

        var result = await query.Get(cancellation, x => x.MovieId == entity.MovieId && x.TagId == entity.TagId);

        return await result.Match(
            entity => Task.FromResult<Result<MovieTag, MovieTagException>>(new MovieTagAlreadyExistsException(entity.MovieId, entity.TagId)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    private async Task<Result<MovieTag, MovieTagException>> CreateEntity(MovieTag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new MovieTagUnknownException(entity.MovieId, entity.TagId, exception);
        }
    }
}
