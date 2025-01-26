using FluentValidation;

namespace Application.Sessions.Commands;

public class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.HallId).NotEmpty();
        RuleFor(x => x.StartAt)
            .NotEmpty()
            .Must(x => x.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("Start should be set after current datetime");

        RuleFor(x => x.EndAt)
            .GreaterThan(x => x.StartAt)
            .When(x => x.EndAt != null);
    }
}
