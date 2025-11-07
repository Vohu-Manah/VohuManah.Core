using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.GetById;

public sealed record GetGapByIdQuery(int Id) : IQuery<GapResponse>;

