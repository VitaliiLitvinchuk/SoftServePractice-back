using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Tickets.Exceptions;
using CSharpFunctionalExtensions;
using Domain.Tickets;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Tickets.Commands;

public class DeleteTicketCommand : IRequest<Result<Ticket, TicketException>>
{
    public required Guid Id { get; init; }
}

public class DeleteTicketCommandHandler(IBaseRepository<Ticket> repository, IBaseQuery<Ticket> query) : IRequestHandler<DeleteTicketCommand, Result<Ticket, TicketException>>
{
    public async Task<Result<Ticket, TicketException>> Handle(DeleteTicketCommand request, CancellationToken cancellation)
    {
        var id = new TicketId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id, include: x => x.Include(x => x.PurchaseHistories));

        return await result.Match(
            async entity =>
            {
                if (entity.PurchaseHistories.Count != 0)
                    return new TicketHasReleationsException(id);

                return await DeleteEntity(entity, cancellation);
            },
            () => Task.FromResult<Result<Ticket, TicketException>>(new TicketNotFoundException(id))
        );
    }

    private async Task<Result<Ticket, TicketException>> DeleteEntity(Ticket entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new TicketUnknownException(entity.Id, exception);
        }
    }
}
