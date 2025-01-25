using FluentValidation;

namespace Application.GenresTags.Commands;

public class DeleteGenreTagCommandValidator : AbstractValidator<DeleteGenreTagCommand>
{
    public DeleteGenreTagCommandValidator()
    {
        RuleFor(x => x.GenreId).NotEmpty();
        RuleFor(x => x.TagId).NotEmpty();
    }
}
