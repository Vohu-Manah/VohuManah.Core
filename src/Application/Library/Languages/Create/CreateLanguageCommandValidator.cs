using FluentValidation;

namespace Application.Library.Languages.Create;

internal sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(30);
        RuleFor(c => c.Abbreviation).NotEmpty().MaximumLength(5);
    }
}


