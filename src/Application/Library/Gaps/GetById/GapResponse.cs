namespace Application.Library.Gaps.GetById;

public sealed record GapResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public bool State { get; init; }
}

