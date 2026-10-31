using FluentValidation;

namespace Application.Library.Settings.UpdateUserSettings;

internal sealed class UpdateUserSettingsCommandValidator : AbstractValidator<UpdateUserSettingsCommand>
{
    public UpdateUserSettingsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر معتبر نیست");

        RuleFor(x => x.ProfileImageAttachmentId)
            .GreaterThan(0).WithMessage("شناسه پیوست عکس پروفایل معتبر نیست")
            .When(x => x.ProfileImageAttachmentId.HasValue);

        RuleFor(x => x.ThemePrimaryColor)
            .MaximumLength(50).WithMessage("رنگ اصلی نمی تواند بیش از ۵۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ThemePrimaryColor));

        RuleFor(x => x.ThemeSecondaryColor)
            .MaximumLength(50).WithMessage("رنگ ثانویه نمی تواند بیش از ۵۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ThemeSecondaryColor));

        RuleFor(x => x.ThemeAccentColor)
            .MaximumLength(50).WithMessage("رنگ تاکیدی نمی تواند بیش از ۵۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ThemeAccentColor));

        RuleFor(x => x.ThemeMode)
            .MaximumLength(20).WithMessage("حالت تم نمی تواند بیش از ۲۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ThemeMode));
    }
}

