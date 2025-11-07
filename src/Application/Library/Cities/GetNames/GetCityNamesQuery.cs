using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.Cities.GetNames;

public sealed record GetCityNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


