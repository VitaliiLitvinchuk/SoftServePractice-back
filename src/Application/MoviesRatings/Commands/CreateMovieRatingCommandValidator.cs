using FluentValidation;

namespace Application.MoviesRatings.Commands;

public class CreateMovieRatingCommandValidator : AbstractValidator<CreateMovieRatingCommand>
{
    public CreateMovieRatingCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating)
            .NotEmpty()
            .InclusiveBetween(1, 10);
    }
}
