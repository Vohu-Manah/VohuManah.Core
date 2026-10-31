using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Languages.GetNames;

public sealed record GetLanguageNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


