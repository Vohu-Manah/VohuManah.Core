using FluentValidation;

namespace Application.Library.Settings.Update;

internal sealed class UpdateSettingsCommandValidator : AbstractValidator<UpdateSettingsCommand>
{
    public UpdateSettingsCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام تنظیمات الزامی است")
            .MaximumLength(100).WithMessage("نام تنظیمات باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Value)
            .NotEmpty().WithMessage("مقدار تنظیمات الزامی است")
            .MaximumLength(500).WithMessage("مقدار تنظیمات باید حداکثر ۵۰۰ کاراکتر باشد");
    }
}

