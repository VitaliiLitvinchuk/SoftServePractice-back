using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.GenresTags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Genres;
using Domain.GenresTags;
using Domain.Tags;
using MediatR;

namespace Application.GenresTags.Commands;

public class DeleteGenreTagCommand : IRequest<Result<GenreTag, GenreTagException>>
{
    public required Guid GenreId { get; init; }
    public required Guid TagId { get; init; }
}

public class DeleteGenreTagCommandHandler(IBaseRepository<GenreTag> repository, IBaseQuery<GenreTag> query) : IRequestHandler<DeleteGenreTagCommand, Result<GenreTag, GenreTagException>>
{
    public async Task<Result<GenreTag, GenreTagException>> Handle(DeleteGenreTagCommand request, CancellationToken cancellation)
    {
        var genreId = new GenreId(request.GenreId);
        var tagId = new TagId(request.TagId);

        var entity = GenreTag.New(genreId, tagId);

        var result = await query.Get(cancellation, x => x.GenreId == entity.GenreId && x.TagId == entity.TagId);

        return await result.Match(
            async entity => await DeleteEntity(entity, cancellation),
            () => Task.FromResult<Result<GenreTag, GenreTagException>>(new GenreTagNotFoundException(entity.GenreId, entity.TagId))
        );
    }

    private async Task<Result<GenreTag, GenreTagException>> DeleteEntity(GenreTag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new GenreTagUnknownException(entity.GenreId, entity.TagId, exception);
        }
    }
}
