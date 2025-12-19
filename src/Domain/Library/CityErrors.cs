using SharedKernel;

namespace Domain.Library;

public static class CityErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Cities.NotFound",
        $"شهر با شناسه '{id}' یافت نشد");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Cities.NameNotUnique",
        "شهری با این نام از قبل وجود دارد");
}

