using SharedKernel;

namespace Domain.Library;

public static class GapErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Gaps.NotFound",
        $"شکاف با شناسه '{id}' یافت نشد");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "Gaps.TitleNotUnique",
        "شکافی با این عنوان از قبل وجود دارد");
}

