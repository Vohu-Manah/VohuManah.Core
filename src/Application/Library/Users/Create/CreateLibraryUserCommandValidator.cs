using FluentValidation;

namespace Application.Library.Users.Create;

internal sealed class CreateLibraryUserCommandValidator : AbstractValidator<CreateLibraryUserCommand>
{
    public CreateLibraryUserCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty().WithMessage("نام کاربری الزامی است")
            .MaximumLength(30).WithMessage("نام کاربری باید حداکثر ۳۰ کاراکتر باشد");
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("رمز عبور الزامی است")
            .MaximumLength(20).WithMessage("رمز عبور باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام الزامی است")
            .MaximumLength(25).WithMessage("نام باید حداکثر ۲۵ کاراکتر باشد");
        RuleFor(c => c.LastName)
            .NotEmpty().WithMessage("نام خانوادگی الزامی است")
            .MaximumLength(50).WithMessage("نام خانوادگی باید حداکثر ۵۰ کاراکتر باشد");
    }
}


