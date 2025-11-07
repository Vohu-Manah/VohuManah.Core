using SharedKernel;

namespace Domain.Library;

public static class LanguageErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Languages.NotFound",
        $"The language with the Id = '{id}' was not found");

    public static readonly Error NameNotUnique = Error.Conflict(
        "Languages.NameNotUnique",
        "A language with this name already exists");
}

