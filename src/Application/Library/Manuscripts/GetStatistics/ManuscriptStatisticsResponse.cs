using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetStatistics;

public sealed record ManuscriptStatisticsResponse
{
    public int ManuscriptCount { get; init; }
    public IReadOnlyList<ListItemResponse> CountBySubject { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByLanguage { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByGap { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByWritingPlace { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByYear { get; init; } = new List<ListItemResponse>();
}

