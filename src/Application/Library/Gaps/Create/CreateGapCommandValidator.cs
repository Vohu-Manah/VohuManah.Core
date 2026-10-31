using FluentValidation;

namespace Application.Library.Gaps.Create;

internal sealed class CreateGapCommandValidator : AbstractValidator<CreateGapCommand>
{
    public CreateGapCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان فاصله الزامی است")
            .MaximumLength(100).WithMessage("عنوان فاصله باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Note)
            .MaximumLength(200).WithMessage("یادداشت باید حداکثر ۲۰۰ کاراکتر باشد");
    }
}

