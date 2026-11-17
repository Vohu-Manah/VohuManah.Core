using VohuManah.SharedKernel;

namespace VohuManah.Domain.Subjects;

public static class SubjectErrors
{
    public static Error NotFound(int id) => Error.NotFound(
        "Subjects.NotFound",
        $"موضوع با شناسه '{id}' یافت نشد");

    public static readonly Error TitleNotUnique = Error.Conflict(
        "Subjects.TitleNotUnique",
        "موضوعی با این عنوان از قبل وجود دارد");
}

