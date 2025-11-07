using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.Create;

public sealed record CreateManuscriptCommand(
    string Name,
    string Author,
    int WritingPlaceId,
    string Year,
    int PageCount,
    string Size,
    int GapId,
    string VersionNo,
    int LanguageId,
    int SubjectId) : ICommand<int>;

