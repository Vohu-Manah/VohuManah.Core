using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.GetById;

internal sealed class GetManuscriptByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetManuscriptByIdQuery, ManuscriptResponse>
{
    public async Task<Result<ManuscriptResponse>> Handle(GetManuscriptByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
        Manuscript? manuscript = await manuscripts
            .Include(m => m.Language)
            .Include(m => m.Subject)
            .Include(m => m.City)
            .Include(m => m.Gap)
            .SingleOrDefaultAsync(m => m.Id == query.Id, cancellationToken);

        if (manuscript == null)
        {
            return Result.Failure<ManuscriptResponse>(ManuscriptErrors.NotFound(query.Id));
        }

        ManuscriptResponse response = new ManuscriptResponse {
            Id = manuscript.Id,
            Name = manuscript.Name,
            Author = manuscript.Author,
            WritingPlaceId = manuscript.WritingPlaceId,
            WritingPlaceName = manuscript.City?.Name,
            Year = manuscript.Year,
            PageCount = manuscript.PageCount,
            Size = manuscript.Size,
            GapId = manuscript.GapId,
            GapTitle = manuscript.Gap?.Title,
            VersionNo = manuscript.VersionNo,
            LanguageId = manuscript.LanguageId,
            LanguageName = manuscript.Language?.Name,
            SubjectId = manuscript.SubjectId,
            SubjectTitle = manuscript.Subject?.Title
        };

        return Result.Success(response);
    }
}

