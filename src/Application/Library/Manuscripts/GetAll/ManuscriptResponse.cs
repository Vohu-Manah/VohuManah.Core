namespace Application.Library.Manuscripts.GetAll;

public sealed record ManuscriptResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string? WritingPlaceName { get; init; }
    public string Year { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public string Size { get; init; } = string.Empty;
    public string? GapTitle { get; init; }
    public string VersionNo { get; init; } = string.Empty;
}

