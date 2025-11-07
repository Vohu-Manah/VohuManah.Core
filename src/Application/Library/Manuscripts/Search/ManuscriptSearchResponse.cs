namespace Application.Library.Manuscripts.Search;

public sealed record ManuscriptSearchResponse
{
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string WritingPlace { get; init; } = string.Empty;
    public string Year { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public string VersionNo { get; init; } = string.Empty;
    public string Gap { get; init; } = string.Empty;
    public string Size { get; init; } = string.Empty;
}

