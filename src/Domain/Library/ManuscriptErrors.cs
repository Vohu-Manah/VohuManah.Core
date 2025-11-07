using SharedKernel;

namespace Domain.Library;

public static class ManuscriptErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Manuscripts.NotFound",
        $"The manuscript with the Id = '{id}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Manuscripts.NameNotUnique",
        "A manuscript with this name already exists");
}

