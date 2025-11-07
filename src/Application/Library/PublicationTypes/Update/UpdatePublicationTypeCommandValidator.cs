using FluentValidation;

namespace Application.Library.PublicationTypes.Update;

internal sealed class UpdatePublicationTypeCommandValidator : AbstractValidator<UpdatePublicationTypeCommand>
{
    public UpdatePublicationTypeCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.Title).NotEmpty().MaximumLength(50);
    }
}

