using FluentValidation;

namespace Application.Library.PublicationTypes.Update;

internal sealed class UpdatePublicationTypeCommandValidator : AbstractValidator<UpdatePublicationTypeCommand>
{
    public UpdatePublicationTypeCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه نوع نشریه نامعتبر است");
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("عنوان نوع نشریه الزامی است")
            .MaximumLength(50).WithMessage("عنوان نوع نشریه باید حداکثر ۵۰ کاراکتر باشد");
    }
}

