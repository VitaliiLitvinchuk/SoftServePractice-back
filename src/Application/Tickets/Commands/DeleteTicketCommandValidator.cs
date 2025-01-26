using FluentValidation;

namespace Application.Tickets.Commands;

public class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
