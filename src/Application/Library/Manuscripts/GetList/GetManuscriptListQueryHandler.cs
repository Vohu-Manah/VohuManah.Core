using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetList;

internal sealed class GetManuscriptListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptListQuery, PagedList<ManuscriptListResponse>>
{
    public async Task<Result<PagedList<ManuscriptListResponse>>> Handle(GetManuscriptListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>()
            .Include(m => m.City)
            .Include(m => m.Gap)
            .Include(m => m.Subject)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Year))
        {
            manuscripts = manuscripts.Where(m => m.Year == query.Year);
        }

        
        if (!string.IsNullOrWhiteSpace(query.FromDate))
        {
            string fromYear = query.FromDate.Length >= 4 ? query.FromDate[..4] : query.FromDate;
            manuscripts = manuscripts.Where(m => m.Year.CompareTo(fromYear) >= 0);
        }

        if (!string.IsNullOrWhiteSpace(query.ToDate))
        {
            string toYear = query.ToDate.Length >= 4 ? query.ToDate[..4] : query.ToDate;
            manuscripts = manuscripts.Where(m => m.Year.CompareTo(toYear) <= 0);
        }

        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            manuscripts = manuscripts.Where(m =>
                m.Name.Contains(term) ||
                m.VersionNo.Contains(term) ||
                (m.Subject != null && m.Subject.Title.Contains(term)));
        }

        int totalCount = await manuscripts.CountAsync(cancellationToken);

        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 200);

        List<ManuscriptListResponse> items = await manuscripts
            .OrderByDescending(m => m.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ManuscriptListResponse
            {
                Id = m.Id,
                Name = m.Name,
                Author = m.Author,
                WritingPlaceName = m.City != null ? m.City.Name : null,
                Year = m.Year,
                PageCount = m.PageCount,
                Size = m.Size,
                GapTitle = m.Gap != null ? m.Gap.Title : null,
                VersionNo = m.VersionNo
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedList<ManuscriptListResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
