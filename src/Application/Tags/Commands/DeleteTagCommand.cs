using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tags;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tags.Commands;

public class DeleteTagCommand : IRequest<Result<Tag, TagException>>
{
    public required Guid Id { get; init; }
}

public class DeleteTagCommandHandler(IBaseRepository<Tag> repository, IBaseQuery<Tag> query) : IRequestHandler<DeleteTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(DeleteTagCommand request, CancellationToken cancellation)
    {
        var id = new TagId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.Movies).Include(x => x.Genres));

        return await result.Match(
            async entity =>
            {
                if (entity.Movies.Count != 0)
                    return new TagHasReleationsException(id);

                if (entity.Genres.Count != 0)
                    return new TagHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Tag, TagException>>(new TagNotFoundException(id))
        );
    }

    private async Task<Result<Tag, TagException>> DeleteEntity(Tag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(entity.Id, exception);
        }
    }
}
