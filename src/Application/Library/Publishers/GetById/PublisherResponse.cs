namespace Application.Library.Publishers.GetById;

public sealed record PublisherResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ManagerName { get; init; } = string.Empty;
    public int PlaceId { get; init; }
    public string Address { get; init; } = string.Empty;
    public string Telephone { get; init; } = string.Empty;
    public string Website { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

