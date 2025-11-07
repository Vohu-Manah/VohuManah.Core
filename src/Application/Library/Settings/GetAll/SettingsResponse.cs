namespace Application.Library.Settings.GetAll;

public sealed record SettingsResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
}

