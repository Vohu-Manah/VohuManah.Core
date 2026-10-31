using FluentValidation;

namespace Application.Library.PublicationTypes.Create;

internal sealed class CreatePublicationTypeCommandValidator : AbstractValidator<CreatePublicationTypeCommand>
{
    public CreatePublicationTypeCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان نوع نشریه الزامی است")
            .MaximumLength(50).WithMessage("عنوان نوع نشریه باید حداکثر ۵۰ کاراکتر باشد");
    }
}

