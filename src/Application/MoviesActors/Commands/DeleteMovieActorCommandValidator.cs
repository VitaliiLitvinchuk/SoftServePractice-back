using FluentValidation;

namespace Application.MoviesActors.Commands;

public class DeleteMovieActorCommandValidator : AbstractValidator<DeleteMovieActorCommand>
{
    public DeleteMovieActorCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.ActorId).NotEmpty();
    }
}
