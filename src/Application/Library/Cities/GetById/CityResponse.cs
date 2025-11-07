namespace Application.Library.Cities.GetById;

public sealed record CityResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

