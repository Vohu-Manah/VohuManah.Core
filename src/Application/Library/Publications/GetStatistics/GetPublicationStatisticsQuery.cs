using Application.Abstractions.Messaging;

namespace Application.Library.Publications.GetStatistics;

public sealed record GetPublicationStatisticsQuery() : IQuery<PublicationStatisticsResponse>;

