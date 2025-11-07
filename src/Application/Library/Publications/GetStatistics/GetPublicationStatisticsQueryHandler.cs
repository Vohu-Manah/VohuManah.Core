using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetStatistics;

internal sealed class GetPublicationStatisticsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationStatisticsQuery, PublicationStatisticsResponse>
{
    public async Task<Result<PublicationStatisticsResponse>> Handle(GetPublicationStatisticsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        int publicationCount = await publications.CountAsync(cancellationToken);

        List<ListItemResponse> countBySubject = await publications
            .Include(p => p.Subject)
            .GroupBy(p => new { p.SubjectId, SubjectTitle = p.Subject != null ? p.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByLanguage = await publications
            .Include(p => p.Language)
            .GroupBy(p => new { p.LanguageId, LanguageName = p.Language != null ? p.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByType = await publications
            .Include(p => p.PublicationType)
            .GroupBy(p => new { p.TypeId, TypeTitle = p.PublicationType != null ? p.PublicationType.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.TypeTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByPublishPlace = await publications
            .Include(p => p.City)
            .GroupBy(p => new { p.PublishPlaceId, CityName = p.City != null ? p.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByPublishDate = await publications
            .GroupBy(p => p.PublishDate)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        PublicationStatisticsResponse response = new PublicationStatisticsResponse {
            PublicationCount = publicationCount,
            CountBySubject = countBySubject,
            CountByLanguage = countByLanguage,
            CountByType = countByType,
            CountByPublishPlace = countByPublishPlace,
            CountByPublishDate = countByPublishDate
        };

        return Result.Success(response);
    }
}

