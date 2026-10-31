using FluentValidation;

namespace Application.Library.Gaps.Update;

internal sealed class UpdateGapCommandValidator : AbstractValidator<UpdateGapCommand>
{
    public UpdateGapCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه فاصله نامعتبر است");
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان فاصله الزامی است")
            .MaximumLength(100).WithMessage("عنوان فاصله باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Note)
            .MaximumLength(200).WithMessage("یادداشت باید حداکثر ۲۰۰ کاراکتر باشد");
    }
}

