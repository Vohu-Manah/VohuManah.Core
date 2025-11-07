using Application.Abstractions.Messaging;

namespace Application.Library.Cities.GetAll;

public sealed record GetAllCitiesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<CityResponse>>;


