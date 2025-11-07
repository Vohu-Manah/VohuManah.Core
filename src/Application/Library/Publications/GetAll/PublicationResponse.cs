namespace Application.Library.Publications.GetAll;

public sealed record PublicationResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Concessionaire { get; init; } = string.Empty;
    public string ResponsibleDirector { get; init; } = string.Empty;
    public string Editor { get; init; } = string.Empty;
    public string Year { get; init; } = string.Empty;
    public string JournalNo { get; init; } = string.Empty;
    public string PublishDate { get; init; } = string.Empty;
    public string? PublishPlaceName { get; init; }
    public string No { get; init; } = string.Empty;
    public string Period { get; init; } = string.Empty;
}

