using SharedKernel;

namespace Domain.Library;

public static class PublisherErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Publishers.NotFound",
        $"ناشر با شناسه '{id}' یافت نشد");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Publishers.NameNotUnique",
        "ناشری با این نام از قبل وجود دارد");
}

