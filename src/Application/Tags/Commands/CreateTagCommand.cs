using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tags.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tags;
using MediatR;

namespace Application.Tags.Commands;

public class CreateTagCommand : IRequest<Result<Tag, TagException>>
{
    public required string Name { get; init; }
}

public class CreateTagCommandHandler(IBaseRepository<Tag> repository, IBaseQuery<Tag> query) : IRequestHandler<CreateTagCommand, Result<Tag, TagException>>
{
    public async Task<Result<Tag, TagException>> Handle(CreateTagCommand request, CancellationToken cancellation)
    {
        var id = TagId.New();
        var entity = Tag.New(id, request.Name);

        var result = await query.Get(cancellation, x => x.Name == entity.Name);

        return await result.Match(
            entity => Task.FromResult<Result<Tag, TagException>>(new TagNameAlreadyExistsException(entity.Id, entity.Name)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    private async Task<Result<Tag, TagException>> CreateEntity(Tag entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TagUnknownException(entity.Id, exception);
        }
    }
}
