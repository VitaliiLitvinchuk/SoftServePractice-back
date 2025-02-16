using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Halls.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Halls;
using Domain.Seats;
using MediatR;

namespace Application.Halls.Commands;

public class UpdateHallCommand : IRequest<Result<Hall, HallException>>
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required short Capacity { get; init; }
}

public class UpdateHallCommandHandler(IBaseRepository<Hall> repository, IBaseQuery<Hall> query, IBaseQuery<Seat> seatQuery) : IRequestHandler<UpdateHallCommand, Result<Hall, HallException>>
{
    public async Task<Result<Hall, HallException>> Handle(UpdateHallCommand request, CancellationToken cancellation)
    {
        var id = new HallId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity =>
            {
                if (entity.Name != request.Name)
                {
                    var result = await query.GetMany(cancellation, x => x.Name == request.Name);

                    if (result.Any())
                    {
                        return new HallNameAlreadyExistsException(id, request.Name);
                    }
                }

                var seats = await seatQuery.GetMany(cancellation, x => x.HallId == id);

                if (seats.Count() >= request.Capacity)
                {

                }


                return await UpdateEntity(entity, request.Name, request.Capacity, cancellation);
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
