using FluentValidation;

namespace Application.Library.Users.Create;

internal sealed class CreateLibraryUserCommandValidator : AbstractValidator<CreateLibraryUserCommand>
{
    public CreateLibraryUserCommandValidator()
    {
        RuleFor(c => c.UserName).NotEmpty().MaximumLength(30);
        RuleFor(c => c.Password).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(25);
        RuleFor(c => c.LastName).NotEmpty().MaximumLength(50);
    }
}


