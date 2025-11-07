namespace Application.Library.Settings.GetById;

public sealed record SettingsResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

