using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Cities.GetById;

public sealed record GetCityByIdQuery(int Id) : IQuery<CityResponse>;

