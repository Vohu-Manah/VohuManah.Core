using FluentValidation;

namespace Application.Library.Manuscripts.Update;

internal sealed class UpdateManuscriptCommandValidator : AbstractValidator<UpdateManuscriptCommand>
{
    public UpdateManuscriptCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه نسخه خطی نامعتبر است");
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام نسخه خطی الزامی است")
            .MaximumLength(100).WithMessage("نام نسخه خطی باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Author)
            .NotEmpty().WithMessage("نام نویسنده الزامی است")
            .MaximumLength(200).WithMessage("نام نویسنده باید حداکثر ۲۰۰ کاراکتر باشد");
        RuleFor(c => c.Size)
            .MaximumLength(100).WithMessage("اندازه باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.VersionNo)
            .MaximumLength(20).WithMessage("شماره نسخه باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.Year)
            .MaximumLength(30).WithMessage("سال باید حداکثر ۳۰ کاراکتر باشد");
        RuleFor(c => c.PageCount)
            .GreaterThan(0).WithMessage("تعداد صفحات باید بزرگتر از صفر باشد");
        RuleFor(c => c.WritingPlaceId)
            .GreaterThan(0).WithMessage("انتخاب محل نگارش الزامی است");
        RuleFor(c => c.GapId)
            .GreaterThan(0).WithMessage("انتخاب فاصله الزامی است");
        RuleFor(c => c.LanguageId)
            .GreaterThan(0).WithMessage("انتخاب زبان الزامی است");
        RuleFor(c => c.SubjectId)
            .GreaterThan(0).WithMessage("انتخاب موضوع الزامی است");
    }
}

