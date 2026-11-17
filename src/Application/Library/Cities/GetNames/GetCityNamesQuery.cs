using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Cities.GetNames;

public sealed record GetCityNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


