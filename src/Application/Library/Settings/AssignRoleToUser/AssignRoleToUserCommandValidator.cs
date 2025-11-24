using FluentValidation;

namespace Application.Library.Settings.AssignRoleToUser;

internal sealed class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
{
    public AssignRoleToUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر باید بزرگتر از صفر باشد");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("شناسه نقش باید بزرگتر از صفر باشد");
    }
}

