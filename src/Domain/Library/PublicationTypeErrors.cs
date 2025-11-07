using SharedKernel;

namespace Domain.Library;

public static class PublicationTypeErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "PublicationTypes.NotFound",
        $"The publication type with the Id = '{id}' was not found");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "PublicationTypes.TitleNotUnique",
        "A publication type with this title already exists");
}

