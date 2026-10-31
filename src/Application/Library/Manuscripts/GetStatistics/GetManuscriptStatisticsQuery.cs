using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetStatistics;

public sealed record GetManuscriptStatisticsQuery() : IQuery<ManuscriptStatisticsResponse>
{
    public int FilterType { get; init; }
}

