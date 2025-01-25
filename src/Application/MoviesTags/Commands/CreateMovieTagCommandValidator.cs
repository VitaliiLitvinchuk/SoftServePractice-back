using FluentValidation;

namespace Application.MoviesTags.Commands;

public class CreateMovieTagCommandValidator : AbstractValidator<CreateMovieTagCommand>
{
    public CreateMovieTagCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}
