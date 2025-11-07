using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.Languages.GetNames;

public sealed record GetLanguageNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


