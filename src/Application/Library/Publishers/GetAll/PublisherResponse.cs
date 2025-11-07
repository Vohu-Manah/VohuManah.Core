namespace Application.Library.Publishers.GetAll;

public sealed record PublisherResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ManagerName { get; init; } = string.Empty;
    public string Telephone { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
}


