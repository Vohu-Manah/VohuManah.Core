using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Publications.GetStatistics;

public sealed record GetPublicationStatisticsQuery() : IQuery<PublicationStatisticsResponse>
{
    public int FilterType { get; init; }
}

