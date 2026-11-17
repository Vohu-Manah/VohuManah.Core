using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Gaps.GetById;

public sealed record GetGapByIdQuery(int Id) : IQuery<GapResponse>;

