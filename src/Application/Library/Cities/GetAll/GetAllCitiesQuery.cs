using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Cities.GetAll;

public sealed record GetAllCitiesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<CityResponse>>;


