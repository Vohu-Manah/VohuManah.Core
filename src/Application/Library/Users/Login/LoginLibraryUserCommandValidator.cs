using FluentValidation;

namespace Application.Library.Users.Login;

internal sealed class LoginLibraryUserCommandValidator : AbstractValidator<LoginLibraryUserCommand>
{
    public LoginLibraryUserCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty().WithMessage("نام کاربری یا ایمیل را بنویسید");
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("رمز عبور را بنویسید");
        RuleFor(c => c.HumanVerifyToken)
            .NotEmpty().WithMessage("نشان دهید که ربات نیستید");
    }
}


