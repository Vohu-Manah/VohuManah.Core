namespace Application.Library.Languages.GetById;

public sealed record LanguageResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
}

