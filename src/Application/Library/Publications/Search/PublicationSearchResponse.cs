namespace Application.Library.Publications.Search;

public sealed record PublicationSearchResponse
{
    public string Name { get; init; } = string.Empty;
    public string TypeName { get; init; } = string.Empty;
    public string Concessionaire { get; init; } = string.Empty;
    public string ResponsibleDirector { get; init; } = string.Empty;
    public string Editor { get; init; } = string.Empty;
    public string Year { get; init; } = string.Empty;
    public string JournalNo { get; init; } = string.Empty;
    public string PublishDate { get; init; } = string.Empty;
    public string PublishPlace { get; init; } = string.Empty;
    public string No { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Period { get; init; } = string.Empty;
}

