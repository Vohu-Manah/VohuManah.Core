using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Gaps.GetAll;

public sealed record GetAllGapsQuery() : IQuery<List<GapResponse>>;

