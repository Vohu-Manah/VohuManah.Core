using FluentValidation;

namespace Application.Library.Attachments.Upload;

internal sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator()
    {
        RuleFor(c => c.EntityType).NotEmpty().MaximumLength(50);
        RuleFor(c => c.EntityId).GreaterThan(0);
        RuleFor(c => c.FileName).NotEmpty().MaximumLength(255);
        RuleFor(c => c.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(c => c.FileSize).GreaterThan(0).LessThanOrEqualTo(10 * 1024 * 1024); // 10 MB
        RuleFor(c => c.FileStream).NotNull();
        RuleFor(c => c.CreatedBy).NotEmpty().MaximumLength(100);
        RuleFor(c => c.Description).MaximumLength(500);
    }
}


