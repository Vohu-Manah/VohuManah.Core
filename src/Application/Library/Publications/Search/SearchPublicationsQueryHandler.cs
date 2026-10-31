using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.Search;

internal sealed class SearchPublicationsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<SearchPublicationsQuery, PagedResult<PublicationSearchResponse>>
{
    public async Task<Result<PagedResult<PublicationSearchResponse>>> Handle(SearchPublicationsQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.GetDbSet<Publication>();

        var queryable = publications.AsQueryable();

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryable = queryable.Where(p => p.Name.Contains(query.Name));
        }
        if (!string.IsNullOrEmpty(query.Concessionaire))
        {
            queryable = queryable.Where(p => p.Concessionaire.Contains(query.Concessionaire));
        }
        if (!string.IsNullOrEmpty(query.Editor))
        {
            queryable = queryable.Where(p => p.Editor.Contains(query.Editor));
        }
        if (!string.IsNullOrEmpty(query.ResponsibleDirector))
        {
            queryable = queryable.Where(p => p.ResponsibleDirector.Contains(query.ResponsibleDirector));
        }
        if (!string.IsNullOrEmpty(query.No))
        {
            queryable = queryable.Where(p => p.No == query.No);
        }
        if (!string.IsNullOrEmpty(query.FromYear))
        {
            queryable = queryable.Where(p => p.Year.CompareTo(query.FromYear) >= 0);
        }
        if (!string.IsNullOrEmpty(query.ToYear))
        {
            queryable = queryable.Where(p => p.Year.CompareTo(query.ToYear) <= 0);
        }
        if (query.PublicationTypeId != 0)
        {
            queryable = queryable.Where(p => p.TypeId == query.PublicationTypeId);
        }
        if (query.PublishPlaceId != 0)
        {
            queryable = queryable.Where(p => p.PublishPlaceId == query.PublishPlaceId);
        }
        if (query.LanguageId != 0)
        {
            queryable = queryable.Where(p => p.LanguageId == query.LanguageId);
        }
        if (query.SubjectId != 0)
        {
            queryable = queryable.Where(p => p.SubjectId == query.SubjectId);
        }
        if (!string.IsNullOrEmpty(query.JournalNo))
        {
            queryable = queryable.Where(p => p.JournalNo == query.JournalNo);
        }
        if (!string.IsNullOrEmpty(query.PublishDate))
        {
            queryable = queryable.Where(p => p.PublishDate == query.PublishDate);
        }
        if (!string.IsNullOrEmpty(query.FromPeriod))
        {
            queryable = queryable.Where(p => p.Period.CompareTo(query.FromPeriod) >= 0);
        }
        if (!string.IsNullOrEmpty(query.ToPeriod))
        {
            queryable = queryable.Where(p => p.Period.CompareTo(query.ToPeriod) <= 0);
        }

        
        
        queryable = queryable.OrderBy(p => p.Name).ThenBy(p => p.Id);

        
        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 50000);

        long totalCount = await queryable.LongCountAsync(cancellationToken);

        var results = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.PublicationType)
            .Include(p => p.Language)
            .Include(p => p.Subject)
            .Include(p => p.City)
            .Select(p => new PublicationSearchResponse
            {
                Name = p.Name,
                No = p.No,
                Concessionaire = p.Concessionaire,
                Editor = p.Editor,
                ResponsibleDirector = p.ResponsibleDirector,
                JournalNo = p.JournalNo,
                Subject = p.Subject != null ? p.Subject.Title : string.Empty,
                PublishDate = p.PublishDate,
                PublishPlace = p.City != null ? p.City.Name : string.Empty,
                Year = p.Year,
                Language = p.Language != null ? p.Language.Name : string.Empty,
                TypeName = p.PublicationType != null ? p.PublicationType.Title : string.Empty,
                Period = p.Period
            })
            .Take(query.MaxResults)
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedResult<PublicationSearchResponse>
        {
            Items = results,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }
}

