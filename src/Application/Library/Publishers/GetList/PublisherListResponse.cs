namespace Application.Library.Publishers.GetList;

public sealed record PublisherListResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ManagerName { get; init; } = string.Empty;
    public string Telephone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}

