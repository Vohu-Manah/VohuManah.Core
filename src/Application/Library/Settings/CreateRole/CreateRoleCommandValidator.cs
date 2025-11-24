using FluentValidation;

namespace Application.Library.Settings.CreateRole;

internal sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام نقش نمی\u200Cتواند خالی باشد")
            .MaximumLength(50).WithMessage("نام نقش باید حداکثر ۵۰ کاراکتر باشد");
    }
}

