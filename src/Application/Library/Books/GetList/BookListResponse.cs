namespace Application.Library.Books.GetList;

public sealed record BookListResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string? PublisherName { get; init; }
    public string Translator { get; init; } = string.Empty;
    public string Corrector { get; init; } = string.Empty;
    public string No { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public int VolumeCount { get; init; }
    public string BookShelfRow { get; init; } = string.Empty;
}

