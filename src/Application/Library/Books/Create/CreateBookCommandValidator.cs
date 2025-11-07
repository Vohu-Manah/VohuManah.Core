using FluentValidation;

namespace Application.Library.Books.Create;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Author).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Editor).MaximumLength(100);
        RuleFor(c => c.Corrector).MaximumLength(100);
        RuleFor(c => c.Isbn).MaximumLength(20);
        RuleFor(c => c.No).NotEmpty().MaximumLength(800);
        RuleFor(c => c.VolumeCount).GreaterThan(0);
        RuleFor(c => c.PublisherId).GreaterThan(0);
        RuleFor(c => c.PublishPlaceId).GreaterThan(0);
        RuleFor(c => c.LanguageId).GreaterThan(0);
        RuleFor(c => c.SubjectId).GreaterThan(0);
        RuleFor(c => c.BookShelfRow).MaximumLength(30);
        RuleFor(c => c.Translator).MaximumLength(200);
    }
}
