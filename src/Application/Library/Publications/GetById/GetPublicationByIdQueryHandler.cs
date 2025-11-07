using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetById;

internal sealed class GetPublicationByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationByIdQuery, PublicationResponse>
{
    public async Task<Result<PublicationResponse>> Handle(GetPublicationByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        Publication? publication = await publications
            .Include(p => p.PublicationType)
            .Include(p => p.Language)
            .Include(p => p.Subject)
            .Include(p => p.City)
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (publication == null)
        {
            return Result.Failure<PublicationResponse>(PublicationErrors.NotFound(query.Id));
        }

        PublicationResponse response = new PublicationResponse {
            Id = publication.Id,
            Name = publication.Name,
            TypeId = publication.TypeId,
            TypeName = publication.PublicationType?.Title,
            Concessionaire = publication.Concessionaire,
            ResponsibleDirector = publication.ResponsibleDirector,
            Editor = publication.Editor,
            Year = publication.Year,
            JournalNo = publication.JournalNo,
            PublishDate = publication.PublishDate,
            PublishPlaceId = publication.PublishPlaceId,
            PublishPlaceName = publication.City?.Name,
            No = publication.No,
            Period = publication.Period,
            LanguageId = publication.LanguageId,
            LanguageName = publication.Language?.Name,
            SubjectId = publication.SubjectId,
            SubjectTitle = publication.Subject?.Title
        };

        return Result.Success(response);
    }
}

