using SharedKernel;

namespace Domain.Library;

public static class ManuscriptErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Manuscripts.NotFound",
        $"نویسه با شناسه '{id}' یافت نشد");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Manuscripts.NameNotUnique",
        "نویسه با این نام از قبل وجود دارد");
}

