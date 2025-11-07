using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetStatistics;

internal sealed class GetManuscriptStatisticsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptStatisticsQuery, ManuscriptStatisticsResponse>
{
    public async Task<Result<ManuscriptStatisticsResponse>> Handle(GetManuscriptStatisticsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
        int manuscriptCount = await manuscripts.CountAsync(cancellationToken);

        List<ListItemResponse> countBySubject = await manuscripts
            .Include(m => m.Subject)
            .GroupBy(m => new { m.SubjectId, SubjectTitle = m.Subject != null ? m.Subject.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.SubjectTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByLanguage = await manuscripts
            .Include(m => m.Language)
            .GroupBy(m => new { m.LanguageId, LanguageName = m.Language != null ? m.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByGap = await manuscripts
            .Include(m => m.Gap)
            .GroupBy(m => m.Gap != null && m.Gap.State)
            .Select(g => new ListItemResponse
            {
                Text = g.Key ? "دارد" : "ندارد",
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByWritingPlace = await manuscripts
            .Include(m => m.City)
            .GroupBy(m => new { m.WritingPlaceId, CityName = m.City != null ? m.City.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.CityName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        List<ListItemResponse> countByYear = await manuscripts
            .GroupBy(m => m.Year)
            .Select(g => new ListItemResponse
            {
                Text = g.Key,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        ManuscriptStatisticsResponse response = new ManuscriptStatisticsResponse {
            ManuscriptCount = manuscriptCount,
            CountBySubject = countBySubject,
            CountByLanguage = countByLanguage,
            CountByGap = countByGap,
            CountByWritingPlace = countByWritingPlace,
            CountByYear = countByYear
        };

        return Result.Success(response);
    }
}

