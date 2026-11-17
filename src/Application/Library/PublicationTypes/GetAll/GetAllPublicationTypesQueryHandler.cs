using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.PublicationTypes;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.GetAll;

internal sealed class GetAllPublicationTypesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllPublicationTypesQuery, List<PublicationTypeResponse>>
{
    private const string PublicationTypeAllKey = "sdp.publicationtype.all";

    public Task<Result<List<PublicationTypeResponse>>> Handle(GetAllPublicationTypesQuery query, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.GetDbSet<PublicationType>();

        List<PublicationType> allPublicationTypes = cacheManager.Retrieve(PublicationTypeAllKey, () => publicationTypes.ToList());

        List<PublicationTypeResponse> response = allPublicationTypes.Select(pt => new PublicationTypeResponse
        {
            Id = pt.Id,
            Title = pt.Title
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

