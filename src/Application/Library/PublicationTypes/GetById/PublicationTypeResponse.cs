namespace Application.Library.PublicationTypes.GetById;

public sealed record PublicationTypeResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
}

