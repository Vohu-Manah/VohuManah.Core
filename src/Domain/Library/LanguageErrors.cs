using SharedKernel;

namespace Domain.Library;

public static class LanguageErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Languages.NotFound",
        $"زبان با شناسه '{id}' یافت نشد");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Languages.NameNotUnique",
        "زبانی با این نام از قبل وجود دارد");
}

