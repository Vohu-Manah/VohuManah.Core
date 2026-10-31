namespace Application.Library.Shared;

public sealed record SelectItemResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}


