using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.Search;

internal sealed class SearchManuscriptsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<SearchManuscriptsQuery, PagedResult<ManuscriptSearchResponse>>
{
    public async Task<Result<PagedResult<ManuscriptSearchResponse>>> Handle(SearchManuscriptsQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.GetDbSet<Manuscript>();

        var queryable = manuscripts.AsQueryable();

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryable = queryable.Where(m => m.Name.Contains(query.Name));
        }
        if (!string.IsNullOrEmpty(query.Author))
        {
            queryable = queryable.Where(m => m.Author.Contains(query.Author));
        }
        if (!string.IsNullOrEmpty(query.No))
        {
            queryable = queryable.Where(m => m.VersionNo == query.No);
        }
        if (!string.IsNullOrEmpty(query.FromYear))
        {
            queryable = queryable.Where(m => m.Year.CompareTo(query.FromYear) >= 0);
        }
        if (!string.IsNullOrEmpty(query.ToYear))
        {
            queryable = queryable.Where(m => m.Year.CompareTo(query.ToYear) <= 0);
        }
        if (!string.IsNullOrEmpty(query.Size))
        {
            queryable = queryable.Where(m => m.Size != null && m.Size.Contains(query.Size));
        }
        if (query.FromPageCount != 0)
        {
            queryable = queryable.Where(m => m.PageCount >= query.FromPageCount);
        }
        if (query.ToPageCount != 0)
        {
            queryable = queryable.Where(m => m.PageCount <= query.ToPageCount);
        }
        if (query.LanguageId != 0)
        {
            queryable = queryable.Where(m => m.LanguageId == query.LanguageId);
        }
        if (query.SubjectId != 0)
        {
            queryable = queryable.Where(m => m.SubjectId == query.SubjectId);
        }
        if (query.WritingPlaceId != 0)
        {
            queryable = queryable.Where(m => m.WritingPlaceId == query.WritingPlaceId);
        }
        if (query.GapId != 0)
        {
            queryable = queryable.Where(m => m.GapId == query.GapId);
        }

        
        
        queryable = queryable.OrderBy(m => m.Name).ThenBy(m => m.Id);

        
        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 50000);

        long totalCount = await queryable.LongCountAsync(cancellationToken);

        var results = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(m => m.Subject)
            .Include(m => m.City)
            .Include(m => m.Language)
            .Include(m => m.Gap)
            .Select(m => new ManuscriptSearchResponse
            {
                Name = m.Name,
                Author = m.Author,
                Subject = m.Subject != null ? m.Subject.Title : string.Empty,
                WritingPlace = m.City != null ? m.City.Name : string.Empty,
                Year = m.Year,
                Language = m.Language != null ? m.Language.Name : string.Empty,
                PageCount = m.PageCount,
                VersionNo = m.VersionNo,
                Gap = m.Gap != null ? m.Gap.Title : string.Empty,
                Size = m.Size ?? string.Empty
            })
            .Take(query.MaxResults)
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedResult<ManuscriptSearchResponse>
        {
            Items = results,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }
}

