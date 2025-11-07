using Application.Library._Shared;

namespace Application.Library.Manuscripts.GetStatistics;

public sealed record ManuscriptStatisticsResponse
{
    public int ManuscriptCount { get; init; }
    public List<ListItemResponse> CountBySubject { get; init; } = new();
    public List<ListItemResponse> CountByLanguage { get; init; } = new();
    public List<ListItemResponse> CountByGap { get; init; } = new();
    public List<ListItemResponse> CountByWritingPlace { get; init; } = new();
    public List<ListItemResponse> CountByYear { get; init; } = new();
}

