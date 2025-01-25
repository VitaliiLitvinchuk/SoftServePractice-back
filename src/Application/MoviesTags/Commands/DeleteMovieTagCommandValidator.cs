using FluentValidation;

namespace Application.MoviesTags.Commands;

public class DeleteMovieTagCommandValidator : AbstractValidator<DeleteMovieTagCommand>
{
    public DeleteMovieTagCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}
