using FluentValidation;

namespace Application.Library.Settings.RemoveRoleFromUser;

internal sealed class RemoveRoleFromUserCommandValidator : AbstractValidator<RemoveRoleFromUserCommand>
{
    public RemoveRoleFromUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("نام کاربری نمی\u200Cتواند خالی باشد");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("شناسه نقش باید بزرگتر از صفر باشد");
    }
}

