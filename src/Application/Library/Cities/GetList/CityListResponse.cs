namespace Application.Library.Cities.GetList;

public sealed record CityListResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

