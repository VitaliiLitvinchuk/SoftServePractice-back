using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tickets.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tickets;
using MediatR;

namespace Application.Tickets.Commands;

public class UpdateTicketCommand : IRequest<Result<Ticket, TicketException>>
{
    public required Guid Id { get; init; }
    public required decimal Price { get; init; }
}

public class UpdateTicketCommandHandler(IBaseRepository<Ticket> repository, IBaseQuery<Ticket> query) : IRequestHandler<UpdateTicketCommand, Result<Ticket, TicketException>>
{
    public async Task<Result<Ticket, TicketException>> Handle(UpdateTicketCommand request, CancellationToken cancellation)
    {
        var id = new TicketId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity => await UpdateEntity(entity, request.Price, cancellation),
            () => Task.FromResult<Result<Ticket, TicketException>>(new TicketNotFoundException(id))
        );
    }

    private async Task<Result<Ticket, TicketException>> UpdateEntity(Ticket entity, decimal price, CancellationToken cancellation)
    {
        try
        {
            entity.UpdateDetails(price);

            return await repository.Update(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TicketUnknownException(entity.Id, exception);
        }
    }
}
