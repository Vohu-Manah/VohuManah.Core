using Application.Library.Settings;
using FluentValidation;

namespace Application.Library.Settings.UpdateRolePermissions;

internal sealed class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
{
    public UpdateRolePermissionsCommandValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("شناسه نقش باید بزرگتر از صفر باشد");

        RuleFor(x => x.EndpointNames)
            .NotNull().WithMessage("لیست دسترسی\u200Cها نمی\u200Cتواند خالی باشد");

        RuleForEach(x => x.EndpointNames!)
            .NotEmpty().WithMessage("نام endpoint نامعتبر است")
            .Must(EndpointCatalog.IsKnownEndpoint)
            .WithMessage("endpoint ارسالی معتبر نیست");
    }
}

