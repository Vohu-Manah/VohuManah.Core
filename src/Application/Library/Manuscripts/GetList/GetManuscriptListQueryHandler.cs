using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetList;

internal sealed class GetManuscriptListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptListQuery, List<ManuscriptListResponse>>
{
    public async Task<Result<List<ManuscriptListResponse>>> Handle(GetManuscriptListQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = unitOfWork.Set<Manuscript>();
        
        var list = await manuscripts
            .Include(m => m.City)
            .Include(m => m.Gap)
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

        return Result.Success(list);
    }
}

