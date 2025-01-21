using FluentValidation;

namespace Application.Actors.Commands;

public class CreateActorCommandValidator : AbstractValidator<CreateActorCommand>
{
    public CreateActorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Middlename)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(1000);
    }
}
