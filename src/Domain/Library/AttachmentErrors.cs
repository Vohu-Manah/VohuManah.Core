using SharedKernel;

namespace Domain.Library;

public static class AttachmentErrors
{
    public static Error NotFound(long id) => Error.NotFound(
        "Attachments.NotFound",
        $"فایل با شناسه '{id}' یافت نشد");

    public static readonly Error FileTooLarge = Error.Validation(
        "Attachments.FileTooLarge",
        "حجم فایل بیش از حد مجاز است");

    public static readonly Error InvalidFileType = Error.Validation(
        "Attachments.InvalidFileType",
        "نوع فایل مجاز نیست");

    public static readonly Error UploadFailed = Error.Failure(
        "Attachments.UploadFailed",
        "آپلود فایل با خطا مواجه شد");

    public static readonly Error DeleteFailed = Error.Failure(
        "Attachments.DeleteFailed",
        "حذف فایل با خطا مواجه شد");
}


