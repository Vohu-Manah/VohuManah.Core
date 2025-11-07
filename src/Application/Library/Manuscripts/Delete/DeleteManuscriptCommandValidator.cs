using FluentValidation;

namespace Application.Library.Manuscripts.Delete;

internal sealed class DeleteManuscriptCommandValidator : AbstractValidator<DeleteManuscriptCommand>
{
    public DeleteManuscriptCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
    }
}

