using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Halls.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using MediatR;

namespace Application.Halls.Commands;

public class CreateHallCommand : IRequest<Result<Hall, HallException>>
{
    public required string Name { get; init; }
    public required short Capacity { get; init; }
}

public class CreateHallCommandHandler(IBaseRepository<Hall> repository, IBaseQuery<Hall> query) : IRequestHandler<CreateHallCommand, Result<Hall, HallException>>
{
    public async Task<Result<Hall, HallException>> Handle(CreateHallCommand request, CancellationToken cancellation)
    {
        var id = HallId.New();
        var entity = Hall.New(id, request.Name, request.Capacity);

        var result = await query.Get(cancellation, x => x.Name == entity.Name);

        return await result.Match(
            entity => Task.FromResult<Result<Hall, HallException>>(new HallNameAlreadyExistsException(entity.Id, entity.Name)),
            async () => await CreateEntity(entity, cancellation)
        );
    }

    private async Task<Result<Hall, HallException>> CreateEntity(Hall entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new HallUnknownException(entity.Id, exception);
        }
    }
}
