namespace Application.Library.Books.Search;

public sealed record BookSearchResponse
{
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string PublisherName { get; init; } = string.Empty;
    public string Translator { get; init; } = string.Empty;
    public string Corrector { get; init; } = string.Empty;
    public string No { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public int VolumeCount { get; init; }
    public string Editor { get; init; } = string.Empty;
    public string PublishPlaceName { get; init; } = string.Empty;
    public string PublishYear { get; init; } = string.Empty;
    public string PublishOrder { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string BookShelfRow { get; init; } = string.Empty;
}
