using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands;

public class UpdateTagCommand : IRequest<Result<Tag, TagException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}

public class UpdateTagCommandHandler(IBaseRepository<Tag> repository, IBaseQuery<Tag> query) : IRequestHandler<UpdateTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(UpdateTagCommand request, CancellationToken cancellation)
    {
        var id = new TagId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
                var result = await query.Get(cancellation, x => x.Name == request.Name);

                return await result.Match(
                  entity => Task.FromResult<Result<Tag, TagException>>(new TagNameAlreadyExistsException(entity.Id, entity.Name)),
                  async () => await UpdateEntity(entity, request.Name, cancellation)
                );
            },
            () => Task.FromResult<Result<Tag, TagException>>(new TagNotFoundException(id))
        );
    }

    private async Task<Result<Tag, TagException>> UpdateEntity(Tag entity, string name, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(entity.Id, exception);
        }
    }
}
