using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.PurchaseHistories.Exceptions;
using CSharpFunctionalExtensions;
using Domain.PurchaseHistories;
using MediatR;

namespace Application.PurchaseHistories.Commands;

public class DeletePurchaseHistoryCommand : IRequest<Result<PurchaseHistory, PurchaseHistoryException>>
{
    public required Guid Id { get; init; }
}

public class DeletePurchaseHistoryCommandHandler(IBaseRepository<PurchaseHistory> repository, IBaseQuery<PurchaseHistory> query) : IRequestHandler<DeletePurchaseHistoryCommand, Result<PurchaseHistory, PurchaseHistoryException>>
{
    public async Task<Result<PurchaseHistory, PurchaseHistoryException>> Handle(DeletePurchaseHistoryCommand request, CancellationToken cancellation)
    {
        var id = new PurchaseHistoryId(request.Id);

        var result = await query.Get(cancellation, x => x.Id == id);

        return await result.Match(
            async entity => await DeleteEntity(entity, cancellation),
            () => Task.FromResult<Result<PurchaseHistory, PurchaseHistoryException>>(new PurchaseHistoryNotFoundException(id))
        );
    }

    private async Task<Result<PurchaseHistory, PurchaseHistoryException>> DeleteEntity(PurchaseHistory entity, CancellationToken cancellation)
    {
        try
        {
            return await repository.Delete(entity, cancellation);
        }
        catch (Exception exception)
        {
            return new PurchaseHistoryUnknownException(entity.Id, exception);
        }
    }
}
