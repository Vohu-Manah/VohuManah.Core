using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Cities.GetList;

public sealed record GetCityListQuery : IQuery<List<CityListResponse>>;

