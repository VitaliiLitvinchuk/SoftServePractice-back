using FluentValidation;

namespace Application.Seats.Commands;

public class CreateSeatCommandValidator : AbstractValidator<CreateSeatCommand>
{
    public CreateSeatCommandValidator()
    {
        RuleFor(x => x.Row)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Number)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.HallId).NotEmpty();
    }
}
