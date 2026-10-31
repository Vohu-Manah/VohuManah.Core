using FluentValidation;

namespace Application.Library.Publications.Update;

internal sealed class UpdatePublicationCommandValidator : AbstractValidator<UpdatePublicationCommand>
{
    public UpdatePublicationCommandValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThan(0).WithMessage("شناسه نشریه نامعتبر است");
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("نام نشریه الزامی است")
            .MaximumLength(100).WithMessage("نام نشریه باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Concessionaire)
            .NotEmpty().WithMessage("امتیازدار الزامی است")
            .MaximumLength(100).WithMessage("امتیازدار باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.ResponsibleDirector)
            .NotEmpty().WithMessage("مدیر مسئول الزامی است")
            .MaximumLength(100).WithMessage("مدیر مسئول باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Editor)
            .NotEmpty().WithMessage("ویراستار الزامی است")
            .MaximumLength(100).WithMessage("ویراستار باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Year)
            .NotEmpty().WithMessage("سال الزامی است")
            .MaximumLength(30).WithMessage("سال باید حداکثر ۳۰ کاراکتر باشد");
        RuleFor(c => c.JournalNo)
            .NotEmpty().WithMessage("شماره مجله الزامی است")
            .MaximumLength(20).WithMessage("شماره مجله باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.PublishDate)
            .NotEmpty().WithMessage("تاریخ انتشار الزامی است")
            .MaximumLength(50).WithMessage("تاریخ انتشار باید حداکثر ۵۰ کاراکتر باشد");
        RuleFor(c => c.No)
            .NotEmpty().WithMessage("شماره الزامی است")
            .MaximumLength(20).WithMessage("شماره باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.Period)
            .MaximumLength(20).WithMessage("دوره باید حداکثر ۲۰ کاراکتر باشد");
        RuleFor(c => c.TypeId)
            .GreaterThan(0).WithMessage("انتخاب نوع نشریه الزامی است");
        RuleFor(c => c.PublishPlaceId)
            .GreaterThan(0).WithMessage("انتخاب محل چاپ الزامی است");
        RuleFor(c => c.LanguageId)
            .GreaterThan(0).WithMessage("انتخاب زبان الزامی است");
        RuleFor(c => c.SubjectId)
            .GreaterThan(0).WithMessage("انتخاب موضوع الزامی است");
    }
}

