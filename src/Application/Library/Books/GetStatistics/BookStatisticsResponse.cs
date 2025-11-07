using Application.Library._Shared;

namespace Application.Library.Books.GetStatistics;

public sealed record BookStatisticsResponse
{
    public int BookCount { get; init; }
    public int VolumeCount { get; init; }
    public List<ListItemResponse> CountBySubject { get; init; } = new();
    public List<ListItemResponse> CountByLanguage { get; init; } = new();
    public List<ListItemResponse> CountByPublisher { get; init; } = new();
    public List<ListItemResponse> CountByPublishPlace { get; init; } = new();
    public List<ListItemResponse> CountByYear { get; init; } = new();
}
