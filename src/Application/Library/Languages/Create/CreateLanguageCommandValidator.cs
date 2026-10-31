using FluentValidation;

namespace Application.Library.Languages.Create;

internal sealed class CreateLanguageCommandValidator : AbstractValidator<CreateLanguageCommand>
{
    public CreateLanguageCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام زبان الزامی است")
            .MaximumLength(30).WithMessage("نام زبان باید حداکثر ۳۰ کاراکتر باشد");
        RuleFor(c => c.Abbreviation)
            .NotEmpty().WithMessage("مخفف الزامی است")
            .MaximumLength(5).WithMessage("مخفف باید حداکثر ۵ کاراکتر باشد");
    }
}


