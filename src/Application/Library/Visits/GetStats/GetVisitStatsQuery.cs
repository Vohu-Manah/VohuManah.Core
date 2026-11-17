using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Visits.GetStats;

public sealed record GetVisitStatsQuery(int Days = 30) : IQuery<VisitStatsResponse>;
