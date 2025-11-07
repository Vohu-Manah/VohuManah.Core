namespace Application.Library.Gaps.GetList;

public sealed record GapListResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string StateTitle { get; init; } = string.Empty;
}

