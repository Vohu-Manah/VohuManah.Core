using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Visits;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Visits.GetStats;

internal sealed class GetVisitStatsQueryHandler(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider) : IQueryHandler<GetVisitStatsQuery, VisitStatsResponse>
{
    private const int MaxDays = 90;

    public async Task<Result<VisitStatsResponse>> Handle(GetVisitStatsQuery query, CancellationToken cancellationToken)
    {
        int days = query.Days <= 0 ? 30 : Math.Min(query.Days, MaxDays);
        DateTime from = dateTimeProvider.UtcNow.Date.AddDays(-(days - 1));

        List<DateTime> stamps = await dbContext.VisitLogs
            .Where(v => v.VisitedAt >= from)
            .Select(v => v.VisitedAt)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var byDay = stamps
            .GroupBy(s => s.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        List<VisitStatItem> items = Enumerable.Range(0, days)
            .Select(offset => from.AddDays(offset))
            .Select(day => new VisitStatItem(
                day.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture),
                byDay.TryGetValue(day, out int count) ? count : 0))
            .ToList();

        return Result.Success(new VisitStatsResponse(items));
    }
}
