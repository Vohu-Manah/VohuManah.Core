using Application.Library.Shared;

namespace Application.Library.Publications.GetStatistics;

public sealed record PublicationStatisticsResponse
{
    public int PublicationCount { get; init; }
    public IReadOnlyList<ListItemResponse> CountBySubject { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByLanguage { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByType { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByPublishPlace { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByPublishDate { get; init; } = new List<ListItemResponse>();
}

