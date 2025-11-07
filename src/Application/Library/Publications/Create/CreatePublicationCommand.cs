using Application.Abstractions.Messaging;

namespace Application.Library.Publications.Create;

public sealed record CreatePublicationCommand(
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
    int SubjectId) : ICommand<int>;

