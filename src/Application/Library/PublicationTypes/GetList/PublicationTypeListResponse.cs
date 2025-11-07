namespace Application.Library.PublicationTypes.GetList;

public sealed record PublicationTypeListResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}

