using FluentValidation;

namespace Application.Library.Gaps.Update;

internal sealed class UpdateGapCommandValidator : AbstractValidator<UpdateGapCommand>
{
    public UpdateGapCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Note).MaximumLength(200);
    }
}

