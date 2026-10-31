using Application.Abstractions.Messaging;

namespace Application.Library.Visits.GetStats;

public sealed record GetVisitStatsQuery(int Days = 30) : IQuery<VisitStatsResponse>;
