using SharedKernel;

namespace Domain.Library;

public static class PublicationErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Publications.NotFound",
        $"The publication with the Id = '{id}' was not found");

    public static readonly Error NoNotUnique = Error.Conflict(
        "Publications.NoNotUnique",
        "A publication with this number already exists");
}

