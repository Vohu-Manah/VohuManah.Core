namespace Application.Library._Shared;

public sealed record ListItemResponse
{
    public string Text { get; init; } = string.Empty;
    public int Value { get; init; }
}

