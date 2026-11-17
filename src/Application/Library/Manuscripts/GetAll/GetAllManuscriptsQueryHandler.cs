using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetAll;

internal sealed class GetAllManuscriptsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllManuscriptsQuery, List<ManuscriptResponse>>
{
    public async Task<Result<List<ManuscriptResponse>>> Handle(GetAllManuscriptsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>();

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

