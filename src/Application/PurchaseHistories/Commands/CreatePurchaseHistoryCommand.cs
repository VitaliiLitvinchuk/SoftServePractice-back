using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.PurchaseHistories.Exceptions;
using CSharpFunctionalExtensions;
using Domain.PurchaseHistories;
using Domain.Tickets;
using Domain.Users;
using MediatR;

namespace Application.PurchaseHistories.Commands;

public class CreatePurchaseHistoryCommand : IRequest<Result<PurchaseHistory, PurchaseHistoryException>>
{
    public required Guid UserId { get; init; }
    public required Guid TicketId { get; init; }
}

public class CreatePurchaseHistoryCommandHandler(IBaseRepository<PurchaseHistory> repository, IBaseQuery<PurchaseHistory> query, IBaseQuery<Ticket> ticketQuery, IBaseQuery<User> userQuery) : IRequestHandler<CreatePurchaseHistoryCommand, Result<PurchaseHistory, PurchaseHistoryException>>
{
    public async Task<Result<PurchaseHistory, PurchaseHistoryException>> Handle(CreatePurchaseHistoryCommand request, CancellationToken cancellation)
    {
        var id = PurchaseHistoryId.New();

        var ticketId = new TicketId(request.TicketId);

        var result = await ticketQuery.Get(cancellation, x => x.Id == ticketId);

        return await result.Match(
            async entity =>
            {
                var userId = new UserId(request.UserId);

                var result = await userQuery.Get(cancellation, x => x.Id == userId);

                return await result.Match(
                    async user =>
                    {
                        var result = await query.Get(cancellation, x => x.TicketId == ticketId && x.UserId == userId);

                        return await result.Match(
                            entity => Task.FromResult<Result<PurchaseHistory, PurchaseHistoryException>>(new PurchaseHistoryAlreadyExistsException(id)),
                            async () =>
                            {
                                var purchaseHistory = PurchaseHistory.New(id, userId, ticketId, DateTime.UtcNow);

                                return await CreateEntity(purchaseHistory, cancellation);
                            }
                        );
                    },
                    () => Task.FromResult<Result<PurchaseHistory, PurchaseHistoryException>>(new UserForPurchaseHistoryNotFoundException(id, userId))
                );
            },
            () => Task.FromResult<Result<PurchaseHistory, PurchaseHistoryException>>(new TicketForPurchaseHistoryNotFoundException(id, ticketId))
        );
    }

    private async Task<Result<PurchaseHistory, PurchaseHistoryException>> CreateEntity(PurchaseHistory entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Create(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new PurchaseHistoryUnknownException(entity.Id, exception);
        }
    }
}
