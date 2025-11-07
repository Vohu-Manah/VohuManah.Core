using FluentValidation;

namespace Application.Library.Settings.Update;

internal sealed class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Value).NotEmpty().MaximumLength(500);
    }
}

