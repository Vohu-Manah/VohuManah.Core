using FluentValidation;

namespace Application.Library.Manuscripts.Create;

internal sealed class CreateManuscriptCommandValidator : AbstractValidator<CreateManuscriptCommand>
{
    public CreateManuscriptCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Author).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Size).MaximumLength(100);
        RuleFor(c => c.VersionNo).MaximumLength(20);
        RuleFor(c => c.Year).MaximumLength(30);
        RuleFor(c => c.PageCount).GreaterThan(0);
        RuleFor(c => c.WritingPlaceId).GreaterThan(0);
        RuleFor(c => c.GapId).GreaterThan(0);
        RuleFor(c => c.LanguageId).GreaterThan(0);
        RuleFor(c => c.SubjectId).GreaterThan(0);
    }
}

