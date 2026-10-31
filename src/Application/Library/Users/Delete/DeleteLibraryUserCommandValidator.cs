using FluentValidation;

namespace Application.Library.Users.Delete;

internal sealed class DeleteLibraryUserCommandValidator : AbstractValidator<DeleteLibraryUserCommand>
{
    public DeleteLibraryUserCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه کاربر الزامی است");
    }
}


