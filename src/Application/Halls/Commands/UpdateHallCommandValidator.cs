using FluentValidation;

namespace Application.Halls.Commands;

public class UpdateHallCommandValidator : AbstractValidator<UpdateHallCommand>
{
    public UpdateHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Capacity)
            .Must(x => x > 0)
            .WithMessage("Capacity must be greater than 0.");
    }
}
