using FluentValidation;

namespace Application.Library.Subjects.Create;

internal sealed class CreateSubjectCommandValidator : AbstractValidator<CreateSubjectCommand>
{
    public CreateSubjectCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان موضوع الزامی است")
            .MaximumLength(50).WithMessage("عنوان موضوع باید حداکثر ۵۰ کاراکتر باشد");
    }
}


