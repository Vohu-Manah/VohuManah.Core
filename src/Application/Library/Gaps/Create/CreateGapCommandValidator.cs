using FluentValidation;

namespace Application.Library.Gaps.Create;

internal sealed class CreateGapCommandValidator : AbstractValidator<CreateGapCommand>
{
    public CreateGapCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Note).MaximumLength(200);
    }
}

