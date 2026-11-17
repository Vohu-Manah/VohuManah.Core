using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Books.GetStatistics;

public sealed record GetBookStatisticsQuery : IQuery<BookStatisticsResponse>
{
    public int FilterType { get; init; } = 1;
}

