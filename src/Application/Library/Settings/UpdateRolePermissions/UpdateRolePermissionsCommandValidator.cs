using FluentValidation;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Application.Library.Settings.UpdateRolePermissions;

internal sealed class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("شناسه نقش باید بزرگتر از صفر باشد");

        RuleFor(x => x.PermissionNames)
            .NotNull().WithMessage("لیست دسترسی\u200Cها نمی\u200Cتواند خالی باشد");

        RuleForEach(x => x.PermissionNames!)
            .NotEmpty().WithMessage("نام endpoint نامعتبر است")
            .Must(PermissionCatalog.IsKnownEndpoint)
            .WithMessage("endpoint ارسالی معتبر نیست");
    }
}

