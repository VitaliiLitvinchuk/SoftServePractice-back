using FluentValidation;

namespace Application.MoviesRatings.Commands;

public class DeleteMovieRatingCommandValidator : AbstractValidator<DeleteMovieRatingCommand>
{
    public DeleteMovieRatingCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
