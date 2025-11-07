using FluentValidation;

namespace Application.Library.Publications.Create;

internal sealed class CreatePublicationCommandValidator : AbstractValidator<CreatePublicationCommand>
{
    public CreatePublicationCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Concessionaire).NotEmpty().MaximumLength(100);
        RuleFor(c => c.ResponsibleDirector).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Editor).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Year).NotEmpty().MaximumLength(30);
        RuleFor(c => c.JournalNo).NotEmpty().MaximumLength(20);
        RuleFor(c => c.PublishDate).NotEmpty().MaximumLength(50);
        RuleFor(c => c.No).NotEmpty().MaximumLength(20);
        RuleFor(c => c.Period).MaximumLength(20);
        RuleFor(c => c.TypeId).GreaterThan(0);
        RuleFor(c => c.PublishPlaceId).GreaterThan(0);
        RuleFor(c => c.LanguageId).GreaterThan(0);
        RuleFor(c => c.SubjectId).GreaterThan(0);
    }
}

