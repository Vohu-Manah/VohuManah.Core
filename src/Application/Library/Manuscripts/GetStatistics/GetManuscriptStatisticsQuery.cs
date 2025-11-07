using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.GetStatistics;

public sealed record GetManuscriptStatisticsQuery() : IQuery<ManuscriptStatisticsResponse>;

