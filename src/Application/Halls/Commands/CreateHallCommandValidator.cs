using FluentValidation;

namespace Application.Halls.Commands;

public class CreateHallCommandValidator : AbstractValidator<CreateHallCommand>
{
    public CreateHallCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Capacity)
            .Must(x => x > 0)
            .WithMessage("Capacity must be greater than 0.");
    }
}
