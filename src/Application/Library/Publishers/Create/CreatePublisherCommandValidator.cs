using FluentValidation;

namespace Application.Library.Publishers.Create;

internal sealed class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Address).NotEmpty().MaximumLength(500);
        RuleFor(c => c.Email).NotEmpty().MaximumLength(100);
        RuleFor(c => c.ManagerName).NotEmpty().MaximumLength(100);
        RuleFor(c => c.PlaceId).GreaterThan(0);
        RuleFor(c => c.Telephone).NotEmpty().MaximumLength(15);
        RuleFor(c => c.Website).NotEmpty().MaximumLength(100);
    }
}


