using FluentValidation;

namespace Application.Sessions.Commands;

public class DeleteSessionCommandValidator : AbstractValidator<DeleteSessionCommand>
{
    public DeleteSessionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
