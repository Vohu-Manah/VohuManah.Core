using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Gaps.GetNames;

public sealed record GetGapNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


