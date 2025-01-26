using FluentValidation;

namespace Application.Tickets.Commands;

public class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();
        RuleFor(x => x.SeatId).NotEmpty();
        RuleFor(x => x.Price)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(1_000_000);
    }
}
