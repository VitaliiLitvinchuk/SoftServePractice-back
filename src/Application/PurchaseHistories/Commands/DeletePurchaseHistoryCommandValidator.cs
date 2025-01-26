using FluentValidation;

namespace Application.PurchaseHistories.Commands;

public class DeletePurchaseHistoryCommandValidator : AbstractValidator<DeletePurchaseHistoryCommand>
{
    public DeletePurchaseHistoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
