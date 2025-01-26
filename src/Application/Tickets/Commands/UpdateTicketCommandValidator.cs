using FluentValidation;

namespace Application.Tickets.Commands;

public class UpdateTicketCommandValidator : AbstractValidator<UpdateTicketCommand>
{
    public UpdateTicketCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(1_000_000);
    }
}
