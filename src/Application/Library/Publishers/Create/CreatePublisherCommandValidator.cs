using FluentValidation;

namespace Application.Library.Publishers.Create;

internal sealed class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام ناشر الزامی است")
            .MaximumLength(100).WithMessage("نام ناشر باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Address)
            .NotEmpty().WithMessage("آدرس الزامی است")
            .MaximumLength(500).WithMessage("آدرس باید حداکثر ۵۰۰ کاراکتر باشد");
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("ایمیل الزامی است")
            .MaximumLength(100).WithMessage("ایمیل باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.ManagerName)
            .NotEmpty().WithMessage("نام مدیر الزامی است")
            .MaximumLength(100).WithMessage("نام مدیر باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.PlaceId)
            .GreaterThan(0).WithMessage("انتخاب شهر الزامی است");
        RuleFor(c => c.Telephone)
            .NotEmpty().WithMessage("تلفن الزامی است")
            .MaximumLength(15).WithMessage("تلفن باید حداکثر ۱۵ کاراکتر باشد");
        RuleFor(c => c.Website)
            .NotEmpty().WithMessage("وبسایت الزامی است")
            .MaximumLength(100).WithMessage("وبسایت باید حداکثر ۱۰۰ کاراکتر باشد");
    }
}


