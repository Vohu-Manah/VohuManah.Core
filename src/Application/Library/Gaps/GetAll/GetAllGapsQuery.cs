using Application.Abstractions.Messaging;

namespace Application.Library.Gaps.GetAll;

public sealed record GetAllGapsQuery() : IQuery<List<GapResponse>>;

