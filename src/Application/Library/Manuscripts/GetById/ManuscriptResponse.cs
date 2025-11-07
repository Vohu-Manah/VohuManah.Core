namespace Application.Library.Manuscripts.GetById;

public sealed record ManuscriptResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int WritingPlaceId { get; init; }
    public string? WritingPlaceName { get; init; }
    public string Year { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public string Size { get; init; } = string.Empty;
    public int GapId { get; init; }
    public string? GapTitle { get; init; }
    public string VersionNo { get; init; } = string.Empty;
    public int LanguageId { get; init; }
    public string? LanguageName { get; init; }
    public int SubjectId { get; init; }
    public string? SubjectTitle { get; init; }
}

