namespace Application.Library.PublicationTypes.GetAll;

public sealed record PublicationTypeResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}

