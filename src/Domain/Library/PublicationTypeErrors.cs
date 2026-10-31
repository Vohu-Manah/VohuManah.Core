using SharedKernel;

namespace Domain.Library;

public static class PublicationTypeErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "PublicationTypes.NotFound",
        $"نوع انتشارات با شناسه '{id}' یافت نشد");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "PublicationTypes.TitleNotUnique",
        "نوع انتشارات با این عنوان از قبل وجود دارد");
}

