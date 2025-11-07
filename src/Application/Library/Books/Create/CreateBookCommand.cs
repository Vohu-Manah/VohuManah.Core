using Application.Abstractions.Messaging;

namespace Application.Library.Books.Create;

public sealed record CreateBookCommand(
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
    string BookShelfRow) : ICommand<long>;

