using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.GetList;

internal sealed class GetPublicationTypeListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetPublicationTypeListQuery, List<PublicationTypeListResponse>>
{
    private const string PublicationTypeAllKey = "sdp.publicationtype.all";

    public Task<Result<List<PublicationTypeListResponse>>> Handle(GetPublicationTypeListQuery query, CancellationToken cancellationToken)
    {
        var publicationTypes = unitOfWork.GetDbSet<PublicationType>();

        var allPublicationTypes = cacheManager.Retrieve(PublicationTypeAllKey, () => publicationTypes.ToList());

        var list = allPublicationTypes.Select(pt => new PublicationTypeListResponse
        {
            Id = pt.Id,
            Title = pt.Title
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

