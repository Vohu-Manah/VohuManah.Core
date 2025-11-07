using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.Update;

public sealed record UpdateManuscriptCommand(
    int Id,
    string Name,
    string Author,
    int WritingPlaceId,
    string Year,
    int PageCount,
    string Size,
    int GapId,
    string VersionNo,
    int LanguageId,
    int SubjectId) : ICommand;

