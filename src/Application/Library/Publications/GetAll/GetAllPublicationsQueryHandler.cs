using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetAll;

internal sealed class GetAllPublicationsQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllPublicationsQuery, List<PublicationResponse>>
{
    public async Task<Result<List<PublicationResponse>>> Handle(GetAllPublicationsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = unitOfWork.Set<Publication>();
        
        List<PublicationResponse> allPublications = await publications
            .Include(p => p.City)
            .Select(p => new PublicationResponse
            {
                Id = p.Id,
                Name = p.Name,
                Concessionaire = p.Concessionaire,
                ResponsibleDirector = p.ResponsibleDirector,
                Editor = p.Editor,
                Year = p.Year,
                JournalNo = p.JournalNo,
                PublishDate = p.PublishDate,
                PublishPlaceName = p.City != null ? p.City.Name : null,
                No = p.No,
                Period = p.Period
            })
            .ToListAsync(cancellationToken);

        return Result.Success(allPublications);
    }
}

