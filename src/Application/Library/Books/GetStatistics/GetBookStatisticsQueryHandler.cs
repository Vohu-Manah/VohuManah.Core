using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetStatistics;

internal sealed class GetBookStatisticsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookStatisticsQuery, BookStatisticsResponse>
{
    public async Task<Result<BookStatisticsResponse>> Handle(GetBookStatisticsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
        int bookCount = await books.CountAsync(cancellationToken);
        int volumeCount = await books.SumAsync(b => (int?)b.VolumeCount, cancellationToken) ?? 0;

        List<ListItemResponse> countBySubject = await books
            .Include(b => b.Subject)
            .GroupBy(b => new { b.SubjectId, SubjectTitle = b.Subject != null ? b.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByLanguage = await books
            .Include(b => b.Language)
            .GroupBy(b => new { b.LanguageId, LanguageName = b.Language != null ? b.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByPublisher = await books
            .Include(b => b.Publisher)
            .GroupBy(b => new { b.PublisherId, PublisherName = b.Publisher != null ? b.Publisher.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.PublisherName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByPublishPlace = await books
            .Include(b => b.City)
            .GroupBy(b => new { b.PublishPlaceId, CityName = b.City != null ? b.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByYear = await books
            .GroupBy(b => b.PublishYear)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        BookStatisticsResponse response = new BookStatisticsResponse
        {
            BookCount = bookCount,
            VolumeCount = volumeCount,
            CountBySubject = countBySubject,
            CountByLanguage = countByLanguage,
            CountByPublisher = countByPublisher,
            CountByPublishPlace = countByPublishPlace,
            CountByYear = countByYear
        };

        return Result.Success(response);
    }
}
