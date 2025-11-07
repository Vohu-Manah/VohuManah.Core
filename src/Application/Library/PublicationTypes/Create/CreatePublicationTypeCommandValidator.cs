using FluentValidation;

namespace Application.Library.PublicationTypes.Create;

internal sealed class CreatePublicationTypeCommandValidator : AbstractValidator<CreatePublicationTypeCommand>
{
    public CreatePublicationTypeCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(50);
    }
}

