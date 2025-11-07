using FluentValidation;

namespace Application.Library.PublicationTypes.Delete;

internal sealed class DeletePublicationTypeCommandValidator : AbstractValidator<DeletePublicationTypeCommand>
{
    public DeletePublicationTypeCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}

