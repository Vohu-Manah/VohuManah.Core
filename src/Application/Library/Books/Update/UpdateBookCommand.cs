using Application.Abstractions.Messaging;

namespace Application.Library.Books.Update;

public sealed record UpdateBookCommand(
    long Id,
    string Name,
    string Author,
    string Translator,
    string Editor,
    string Corrector,
    int PublisherId,
    int PublishPlaceId,
    string PublishYear,
    string PublishOrder,
    string Isbn,
    string No,
    int VolumeCount,
    int LanguageId,
    int SubjectId,
    string BookShelfRow) : ICommand;

