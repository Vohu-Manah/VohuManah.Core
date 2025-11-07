using Application.Abstractions.Messaging;

namespace Application.Library.Publications.Update;

public sealed record UpdatePublicationCommand(
    int Id,
    string Name,
    int TypeId,
    string Concessionaire,
    string ResponsibleDirector,
    string Editor,
    string Year,
    string JournalNo,
    string PublishDate,
    int PublishPlaceId,
    string No,
    string Period,
    int LanguageId,
    int SubjectId) : ICommand;

