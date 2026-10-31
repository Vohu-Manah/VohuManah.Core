using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Cities.GetNames;

public sealed record GetCityNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


