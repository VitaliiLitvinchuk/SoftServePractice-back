using FluentValidation;

namespace Application.MoviesActors.Commands;

public class CreateMovieActorCommandValidator : AbstractValidator<CreateMovieActorCommand>
{
    public CreateMovieActorCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.ActorId).NotEmpty();
    }
}
