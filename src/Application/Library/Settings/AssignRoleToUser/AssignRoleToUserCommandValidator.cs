using FluentValidation;

namespace Application.Library.Settings.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("نام کاربری نمی\u200Cتواند خالی باشد");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("شناسه نقش باید بزرگتر از صفر باشد");
    }
}

