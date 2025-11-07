using Application.Abstractions.Messaging;

namespace Application.Library.Cities.GetList;

public sealed record GetCityListQuery : IQuery<List<CityListResponse>>;

