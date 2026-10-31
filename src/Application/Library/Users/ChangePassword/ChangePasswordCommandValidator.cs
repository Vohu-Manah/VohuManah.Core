using FluentValidation;

namespace Application.Library.Users.ChangePassword;

internal sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر معتبر نیست");

        RuleFor(c => c.CurrentPassword)
            .NotEmpty().WithMessage("رمز عبور فعلی الزامی است");

        RuleFor(c => c.NewPassword)
            .NotEmpty().WithMessage("رمز عبور جدید الزامی است")
            .MinimumLength(8).WithMessage("رمز عبور باید حداقل ۸ کاراکتر باشد")
            .MaximumLength(50).WithMessage("رمز عبور باید حداکثر ۵۰ کاراکتر باشد")
            .Must(BeComplexPassword).WithMessage("رمز عبور باید شامل حروف بزرگ، حروف کوچک، عدد و کاراکتر خاص باشد");
    }

    private static bool BeComplexPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}

