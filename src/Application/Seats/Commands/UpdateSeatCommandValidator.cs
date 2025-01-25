using FluentValidation;

namespace Application.Seats.Commands;

public class UpdateSeatCommandValidator : AbstractValidator<UpdateSeatCommand>
{
    public UpdateSeatCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Row)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Number)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.HallId).NotEmpty();
    }
}
