using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetList;

internal sealed class GetPublicationListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationListQuery, PagedList<PublicationListResponse>>
{
    public async Task<Result<PagedList<PublicationListResponse>>> Handle(GetPublicationListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Publication> publications = unitOfWork.GetDbSet<Publication>()
            .Include(p => p.City)
            .Include(p => p.PublicationType)
            .Include(p => p.Subject)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Year))
        {
            publications = publications.Where(p => p.Year == query.Year);
        }

        
        
        if (!string.IsNullOrWhiteSpace(query.FromDate))
        {
            publications = publications.Where(p => p.PublishDate.CompareTo(query.FromDate) >= 0);
        }

        if (!string.IsNullOrWhiteSpace(query.ToDate))
        {
            publications = publications.Where(p => p.PublishDate.CompareTo(query.ToDate) <= 0);
        }

        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            publications = publications.Where(p =>
                p.Name.Contains(term) ||
                p.No.Contains(term) ||
                p.JournalNo.Contains(term) ||
                (p.City != null && p.City.Name.Contains(term)) ||
                (p.PublicationType != null && p.PublicationType.Title.Contains(term)) ||
                (p.Subject != null && p.Subject.Title.Contains(term)));
        }

        int totalCount = await publications.CountAsync(cancellationToken);

        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 200);

        List<PublicationListResponse> items = await publications
            .OrderByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PublicationListResponse
            {
                Id = p.Id,
                Name = p.Name,
                Concessionaire = p.Concessionaire,
                ResponsibleDirector = p.ResponsibleDirector,
                Editor = p.Editor,
                Year = p.Year,
                JournalNo = p.JournalNo,
                PublishDate = p.PublishDate,
                TypeName = p.PublicationType != null ? p.PublicationType.Title : null,
                PublishPlaceName = p.City != null ? p.City.Name : null,
                No = p.No,
                Period = p.Period
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedList<PublicationListResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
