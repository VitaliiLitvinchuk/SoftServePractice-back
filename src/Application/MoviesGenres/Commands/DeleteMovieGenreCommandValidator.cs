using FluentValidation;

namespace Application.MoviesGenres.Commands;

public class DeleteMovieGenreCommandValidator : AbstractValidator<DeleteMovieGenreCommand>
{
    public DeleteMovieGenreCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.GenreId).NotEmpty();
    }
}
