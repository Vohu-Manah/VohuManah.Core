using FluentValidation;

namespace Application.Library.Users.Update;

internal sealed class UpdateLibraryUserCommandValidator : AbstractValidator<UpdateLibraryUserCommand>
{
    public UpdateLibraryUserCommandValidator()
    {
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(30);
        RuleFor(c => c.Password).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(25);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
    }
}


