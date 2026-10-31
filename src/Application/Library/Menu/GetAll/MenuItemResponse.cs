namespace Application.Library.Menu.GetAll;

public sealed record MenuItemResponse
{
    public int Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string? Path { get; init; }
    public string? Icon { get; init; }
    public int? ParentId { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsActive { get; init; }
    public bool IsSystem { get; init; }
    public List<MenuItemResponse>? Children { get; init; }
}

