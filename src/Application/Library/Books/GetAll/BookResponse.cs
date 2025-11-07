namespace Application.Library.Books.GetAll;

public sealed record BookResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Translator { get; init; } = string.Empty;
    public string Editor { get; init; } = string.Empty;
    public string Corrector { get; init; } = string.Empty;
    public int PublisherId { get; init; }
    public string? PublisherName { get; init; }
    public int PublishPlaceId { get; init; }
    public string? PublishPlaceName { get; init; }
    public string PublishYear { get; init; } = string.Empty;
    public string PublishOrder { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public string No { get; init; } = string.Empty;
    public int VolumeCount { get; init; }
    public int LanguageId { get; init; }
    public string? LanguageName { get; init; }
    public int SubjectId { get; init; }
    public string? SubjectTitle { get; init; }
    public string BookShelfRow { get; init; } = string.Empty;
}

