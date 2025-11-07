using FluentValidation;

namespace Application.Library.Users.Login;

internal sealed class LoginLibraryUserCommandValidator : AbstractValidator<LoginLibraryUserCommand>
{
    public LoginLibraryUserCommandValidator()
    {
        RuleFor(c => c.UserName).NotEmpty();
        RuleFor(c => c.Password).NotEmpty();
    }
}


