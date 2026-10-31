namespace Application.Library.Shared;

public sealed record ListItemResponse
{
    public string Text { get; init; } = string.Empty;

    
    public long Value { get; init; }
}
