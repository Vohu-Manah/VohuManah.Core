using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.GetNames;

public sealed record GetGapNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


