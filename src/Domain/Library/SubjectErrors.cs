using SharedKernel;

namespace Domain.Library;

public static class SubjectErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Subjects.NotFound",
        $"The subject with the Id = '{id}' was not found");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "Subjects.TitleNotUnique",
        "A subject with this title already exists");
}

