using SharedKernel;

namespace Domain.Library;

public static class GapErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Gaps.NotFound",
        $"فاصله با شناسه '{id}' یافت نشد");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "Gaps.TitleNotUnique",
        "فواصلی با این عنوان از قبل وجود دارد");
}

