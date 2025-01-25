using FluentValidation;

namespace Application.Seats.Commands;

public class DeleteSeatCommandValidator : AbstractValidator<DeleteSeatCommand>
{
    public DeleteSeatCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
