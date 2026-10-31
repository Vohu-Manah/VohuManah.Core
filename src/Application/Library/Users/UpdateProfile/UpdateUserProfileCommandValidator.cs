using FluentValidation;

namespace Application.Library.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(c => c.UserId)
            .GreaterThan(0).WithMessage("شناسه کاربر معتبر نیست");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام الزامی است")
            .MaximumLength(25).WithMessage("نام باید حداکثر ۲۵ کاراکتر باشد");

        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("نام خانوادگی الزامی است")
            .MaximumLength(50).WithMessage("نام خانوادگی باید حداکثر ۵۰ کاراکتر باشد");

        RuleFor(c => c.Email)
            .EmailAddress().WithMessage("ایمیل معتبر نیست")
            .MaximumLength(100).WithMessage("ایمیل باید حداکثر ۱۰۰ کاراکتر باشد")
            .When(c => !string.IsNullOrEmpty(c.Email));

        RuleFor(c => c.PhoneNumber)
            .MaximumLength(20).WithMessage("شماره تلفن باید حداکثر ۲۰ کاراکتر باشد")
            .When(c => !string.IsNullOrEmpty(c.PhoneNumber));
    }
}

