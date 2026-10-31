namespace Application.Library.Menu.Shared;




public sealed record DefaultMenuItem(
    string Label,
    string? Path,
    string? Icon,
    bool IsSystem,
    int DisplayOrder,
    IReadOnlyList<DefaultMenuItem> Children);