using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Halls.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using MediatR;

namespace Application.Halls.Commands;

public class UpdateHallCommand : IRequest<Result<Hall, HallException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required short Capacity { get; init; }
}

public class UpdateHallCommandHandler(IBaseRepository<Hall> repository, IBaseQuery<Hall> query) : IRequestHandler<UpdateHallCommand, Result<Hall, HallException>>
{
    public async Task<Result<Hall, HallException>> Handle(UpdateHallCommand request, CancellationToken cancellation)
    {
        var id = new HallId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
                var result = await query.Get(cancellation, x => x.Name == request.Name);

                return await result.Match(
                  entity => Task.FromResult<Result<Hall, HallException>>(new HallNameAlreadyExistsException(entity.Id, entity.Name)),
                  async () => await UpdateEntity(entity, request.Name, request.Capacity, cancellation)
                );
            },
            () => Task.FromResult<Result<Hall, HallException>>(new HallNotFoundException(id))
        );
    }

    private async Task<Result<Hall, HallException>> UpdateEntity(Hall entity, string name, short capacity, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(name, capacity);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new HallUnknownException(entity.Id, exception);
        }
    }
}
