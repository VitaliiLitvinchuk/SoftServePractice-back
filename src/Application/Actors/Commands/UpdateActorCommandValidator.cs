using FluentValidation;

namespace Application.Actors.Commands;

public class UpdateActorCommandValidator : AbstractValidator<UpdateActorCommand>
{
    public UpdateActorCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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
