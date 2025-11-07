using Application.Library._Shared;

namespace Application.Library.Publications.GetStatistics;

public sealed record PublicationStatisticsResponse
{
    public int PublicationCount { get; init; }
    public List<ListItemResponse> CountBySubject { get; init; } = new();
    public List<ListItemResponse> CountByLanguage { get; init; } = new();
    public List<ListItemResponse> CountByType { get; init; } = new();
    public List<ListItemResponse> CountByPublishPlace { get; init; } = new();
    public List<ListItemResponse> CountByPublishDate { get; init; } = new();
}

