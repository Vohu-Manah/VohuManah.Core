using FluentValidation;

namespace Application.Library.Gaps.Delete;

internal sealed class DeleteGapCommandValidator : AbstractValidator<DeleteGapCommand>
{
    public DeleteGapCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}

