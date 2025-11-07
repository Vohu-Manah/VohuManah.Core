using SharedKernel;

namespace Domain.Library;

public static class CityErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Cities.NotFound",
        $"The city with the Id = '{id}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Cities.NameNotUnique",
        "A city with this name already exists");
}

