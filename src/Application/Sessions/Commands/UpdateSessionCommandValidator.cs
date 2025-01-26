using FluentValidation;

namespace Application.Sessions.Commands;

public class UpdateSessionCommandValidator : AbstractValidator<UpdateSessionCommand>
{
    public UpdateSessionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.HallId).NotEmpty();
        RuleFor(x => x.StatusId).NotEmpty();
        RuleFor(x => x.StartAt)
            .NotEmpty()
            .Must(x => x.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("Start should be set after current datetime");

        RuleFor(x => x.EndAt)
            .NotEmpty()
            .GreaterThan(x => x.StartAt);
    }
}
