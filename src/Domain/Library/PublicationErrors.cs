using SharedKernel;

namespace Domain.Library;

public static class PublicationErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Publications.NotFound",
        $"انتشار با شناسه '{id}' یافت نشد");

    public static readonly Error NoNotUnique = Error.Conflict(
        "Publications.NoNotUnique",
        "انتشار با این شماره از قبل وجود دارد");
}

