using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetAll;

internal sealed class GetAllManuscriptsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllManuscriptsQuery, List<ManuscriptResponse>>
{
    public async Task<Result<List<ManuscriptResponse>>> Handle(GetAllManuscriptsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
        List<ManuscriptResponse> allManuscripts = await manuscripts
            .Include(m => m.City)
            .Include(m => m.Gap)
            .Select(m => new ManuscriptResponse
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

        return Result.Success(allManuscripts);
    }
}

