using FluentValidation;

namespace Application.Library.Books.Update;

internal sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه کتاب نامعتبر است");
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام کتاب الزامی است")
            .MaximumLength(100).WithMessage("نام کتاب باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Author)
            .NotEmpty().WithMessage("نام نویسنده الزامی است")
            .MaximumLength(200).WithMessage("نام نویسنده باید حداکثر ۲۰۰ کاراکتر باشد");
        RuleFor(c => c.Editor)
            .MaximumLength(100).WithMessage("نام ویراستار باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Corrector)
            .MaximumLength(100).WithMessage("نام مصحح باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Isbn)
            .MaximumLength(20).WithMessage("شابک باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.No)
            .NotEmpty().WithMessage("شماره کتاب الزامی است")
            .MaximumLength(800).WithMessage("شماره کتاب باید حداکثر ۸۰۰ کاراکتر باشد");
        RuleFor(c => c.VolumeCount)
            .GreaterThan(0).WithMessage("تعداد جلد باید بزرگتر از صفر باشد");
        RuleFor(c => c.PublisherId)
            .GreaterThan(0).WithMessage("انتخاب ناشر الزامی است");
        RuleFor(c => c.PublishPlaceId)
            .GreaterThan(0).WithMessage("انتخاب محل چاپ الزامی است");
        RuleFor(c => c.LanguageId)
            .GreaterThan(0).WithMessage("انتخاب زبان الزامی است");
        RuleFor(c => c.SubjectId)
            .GreaterThan(0).WithMessage("انتخاب موضوع الزامی است");
        RuleFor(c => c.BookShelfRow)
            .MaximumLength(30).WithMessage("ردیف قفسه باید حداکثر ۳۰ کاراکتر باشد");
        RuleFor(c => c.Translator)
            .MaximumLength(200).WithMessage("نام مترجم باید حداکثر ۲۰۰ کاراکتر باشد");
    }
}

