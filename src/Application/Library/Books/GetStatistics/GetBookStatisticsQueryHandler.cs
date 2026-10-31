using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetStatistics;

internal sealed class GetBookStatisticsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookStatisticsQuery, BookStatisticsResponse>
{
    public async Task<Result<BookStatisticsResponse>> Handle(GetBookStatisticsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.GetDbSet<Book>();

        
        
        var grouped = await books
            .GroupBy(b => new
            {
                b.SubjectId,
                b.LanguageId,
                b.PublisherId,
                b.PublishPlaceId,
                b.PublishYear,
            })
            .Select(g => new
            {
                g.Key.SubjectId,
                g.Key.LanguageId,
                g.Key.PublisherId,
                g.Key.PublishPlaceId,
                g.Key.PublishYear,
                Count = g.LongCount(),
                VolumeSum = g.Sum(b => (long)b.VolumeCount),
            })
            .ToListAsync(cancellationToken);

        long bookCount = grouped.Sum(g => g.Count);
        long volumeCount = grouped.Sum(g => g.VolumeSum);

        
        var subjectNames = await unitOfWork.GetDbSet<Subject>()
            .ToDictionaryAsync(s => s.Id, s => s.Title, cancellationToken);
        var languageNames = await unitOfWork.GetDbSet<Language>()
            .ToDictionaryAsync(l => l.Id, l => l.Name, cancellationToken);
        var publisherNames = await unitOfWork.GetDbSet<Publisher>()
            .ToDictionaryAsync(p => p.Id, p => p.Name, cancellationToken);
        var cityNames = await unitOfWork.GetDbSet<City>()
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return Result.Success(new BookStatisticsResponse
        {
            BookCount = (int)Math.Min(bookCount, int.MaxValue),
            VolumeCount = (int)Math.Min(volumeCount, int.MaxValue),
            CountBySubject = Aggregate(grouped.Select(g => (g.SubjectId, g.Count)),
                id => subjectNames.TryGetValue(id, out string? n) ? n : string.Empty),
            CountByLanguage = Aggregate(grouped.Select(g => (g.LanguageId, g.Count)),
                id => languageNames.TryGetValue(id, out string? n) ? n : string.Empty),
            CountByPublisher = Aggregate(grouped.Select(g => (g.PublisherId, g.Count)),
                id => publisherNames.TryGetValue(id, out string? n) ? n : string.Empty),
            CountByPublishPlace = Aggregate(grouped.Select(g => (g.PublishPlaceId, g.Count)),
                id => cityNames.TryGetValue(id, out string? n) ? n : string.Empty),
            CountByYear = Aggregate(grouped.Select(g => (g.PublishYear, g.Count)),
                id => id),
        });
    }

    private static List<ListItemResponse> Aggregate<TKey>(
        IEnumerable<(TKey Key, long Count)> source,
        Func<TKey, string> nameResolver)
    {
        return source
            .GroupBy(x => x.Key)
            .Select(g => new ListItemResponse
            {
                Text = nameResolver(g.Key),
                Value = g.Sum(x => x.Count),
            })
            .OrderByDescending(x => x.Value)
            .ToList();
    }
}
