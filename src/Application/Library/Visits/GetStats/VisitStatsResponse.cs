namespace Application.Library.Visits.GetStats;

public sealed record VisitStatsResponse(IReadOnlyList<VisitStatItem> Items);
