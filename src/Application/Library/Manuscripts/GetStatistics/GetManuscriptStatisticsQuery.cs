using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Manuscripts.GetStatistics;

public sealed record GetManuscriptStatisticsQuery() : IQuery<ManuscriptStatisticsResponse>
{
    public int FilterType { get; init; }
}

