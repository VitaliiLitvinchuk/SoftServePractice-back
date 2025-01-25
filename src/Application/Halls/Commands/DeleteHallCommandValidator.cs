using FluentValidation;

namespace Application.Halls.Commands;

public class DeleteHallCommandValidator : AbstractValidator<DeleteHallCommand>
{
    public DeleteHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
