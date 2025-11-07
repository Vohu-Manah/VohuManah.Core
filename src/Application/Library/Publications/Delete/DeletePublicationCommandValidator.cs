using FluentValidation;

namespace Application.Library.Publications.Delete;

internal sealed class DeletePublicationCommandValidator : AbstractValidator<DeletePublicationCommand>
{
    public DeletePublicationCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}

