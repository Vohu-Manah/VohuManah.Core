using FluentValidation;

namespace Application.Library.Attachments.Upload;

internal sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator()
    {
        RuleFor(c => c.EntityType)
            .NotEmpty().WithMessage("نوع موجودیت الزامی است")
            .MaximumLength(50).WithMessage("نوع موجودیت باید حداکثر ۵۰ کاراکتر باشد");
        RuleFor(c => c.EntityId)
            .GreaterThan(0).WithMessage("شناسه موجودیت باید بزرگتر از صفر باشد");
        RuleFor(c => c.FileName)
            .NotEmpty().WithMessage("نام فایل الزامی است")
            .MaximumLength(255).WithMessage("نام فایل باید حداکثر ۲۵۵ کاراکتر باشد");
        RuleFor(c => c.ContentType)
            .NotEmpty().WithMessage("نوع محتوا الزامی است")
            .MaximumLength(100).WithMessage("نوع محتوا باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.FileSize)
            .GreaterThan(0).WithMessage("حجم فایل باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("حجم فایل باید حداکثر ۱۰ مگابایت باشد");
        RuleFor(c => c.FileStream)
            .NotNull().WithMessage("فایل الزامی است");
        RuleFor(c => c.CreatedBy)
            .NotEmpty().WithMessage("ایجادکننده الزامی است")
            .MaximumLength(100).WithMessage("ایجادکننده باید حداکثر ۱۰۰ کاراکتر باشد");
        RuleFor(c => c.Description)
            .MaximumLength(500).WithMessage("توضیحات باید حداکثر ۵۰۰ کاراکتر باشد");
    }
}


