using SharedKernel;

namespace Domain.Library;

public static class PublisherErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Publishers.NotFound",
        $"The publisher with the Id = '{id}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Publishers.NameNotUnique",
        "A publisher with this name already exists");
}

