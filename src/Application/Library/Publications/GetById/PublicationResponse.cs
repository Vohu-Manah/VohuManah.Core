namespace Application.Library.Publications.GetById;

public sealed record PublicationResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int TypeId { get; init; }
    public string? TypeName { get; init; }
    public string Concessionaire { get; init; } = string.Empty;
    public string ResponsibleDirector { get; init; } = string.Empty;
    public string Editor { get; init; } = string.Empty;
    public string Year { get; init; } = string.Empty;
    public string JournalNo { get; init; } = string.Empty;
    public string PublishDate { get; init; } = string.Empty;
    public int PublishPlaceId { get; init; }
    public string? PublishPlaceName { get; init; }
    public string No { get; init; } = string.Empty;
    public string Period { get; init; } = string.Empty;
    public int LanguageId { get; init; }
    public string? LanguageName { get; init; }
    public int SubjectId { get; init; }
    public string? SubjectTitle { get; init; }
}

