using FluentValidation;

namespace Application.GenresTags.Commands;

public class CreateGenreTagCommandValidator : AbstractValidator<CreateGenreTagCommand>
{
    public CreateGenreTagCommandValidator()
    {
        RuleFor(x => x.GenreId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}
