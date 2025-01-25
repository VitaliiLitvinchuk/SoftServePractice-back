using FluentValidation;

namespace Application.Movies.Commands;

public class UpdateMovieCommandValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.Duration)
            .Must(x => x > 0)
            .WithMessage("Duration must be greater than 0.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .MaximumLength(510);

        RuleFor(x => x.TrailerUrl)
            .NotEmpty()
            .MaximumLength(510);

        RuleFor(x => x.ReleaseDate)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(3000);
    }
}
