using Application.Abstractions.Messaging;

namespace Application.Library.Cities.GetById;

public sealed record GetCityByIdQuery(int Id) : IQuery<CityResponse>;

