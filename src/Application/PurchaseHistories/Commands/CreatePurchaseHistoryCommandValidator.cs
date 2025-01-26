using FluentValidation;

namespace Application.PurchaseHistories.Commands;

public class CreatePurchaseHistoryCommandValidator : AbstractValidator<CreatePurchaseHistoryCommand>
{
    public CreatePurchaseHistoryCommandValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
