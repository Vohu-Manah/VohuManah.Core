using Application.Library.Shared;

namespace Application.Library.Books.GetStatistics;

public sealed record BookStatisticsResponse
{
    public int BookCount { get; init; }
    public int VolumeCount { get; init; }
    public IReadOnlyList<ListItemResponse> CountBySubject { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByLanguage { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByPublisher { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByPublishPlace { get; init; } = new List<ListItemResponse>();
    public IReadOnlyList<ListItemResponse> CountByYear { get; init; } = new List<ListItemResponse>();
}
