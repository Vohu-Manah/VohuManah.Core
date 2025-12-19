using SharedKernel;

namespace Domain.Library;

public static class BookErrors
{
    public static Error NotFound(long id) => Error.NotFound(
        "Books.NotFound",
        $"کتاب با شناسه '{id}' یافت نشد");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Books.NameNotUnique",
        "کتابی با این نام از قبل وجود دارد");
}


