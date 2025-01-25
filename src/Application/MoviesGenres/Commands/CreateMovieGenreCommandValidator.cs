using FluentValidation;

namespace Application.MoviesGenres.Commands;

public class CreateMovieGenreCommandValidator : AbstractValidator<CreateMovieGenreCommand>
{
    public CreateMovieGenreCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.GenreId).NotEmpty();
    }
}
