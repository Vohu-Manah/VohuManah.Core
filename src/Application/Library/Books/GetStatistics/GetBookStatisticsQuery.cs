using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetStatistics;

public sealed record GetBookStatisticsQuery() : IQuery<BookStatisticsResponse>;

