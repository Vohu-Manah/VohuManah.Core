namespace Application.Library.Languages.GetList;

public sealed record LanguageListResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Abbreviation { get; init; } = string.Empty;
}

