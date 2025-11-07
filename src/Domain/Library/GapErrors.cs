using SharedKernel;

namespace Domain.Library;

public static class GapErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Gaps.NotFound",
        $"The gap with the Id = '{id}' was not found");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "Gaps.TitleNotUnique",
        "A gap with this title already exists");
}

