using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetList;

internal sealed class GetPublicationListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationListQuery, List<PublicationListResponse>>
{
    public async Task<Result<List<PublicationListResponse>>> Handle(GetPublicationListQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
        var list = await publications
            .Include(p => p.City)
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
                PublishPlaceName = p.City != null ? p.City.Name : null,
                No = p.No,
                Period = p.Period
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

