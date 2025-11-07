namespace Application.Library.Cities.GetAll;

public sealed record CityResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}


