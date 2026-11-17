using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Gaps.GetNames;

public sealed record GetGapNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


