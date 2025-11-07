using FluentValidation;

namespace Application.Library.Subjects.Create;

internal sealed class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(50);
    }
}


