using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetAllEntities;

internal sealed class GetAllSettingsEntitiesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllSettingsEntitiesQuery, List<Domain.Library.Settings>>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<List<Domain.Library.Settings>>> Handle(GetAllSettingsEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        
        List<Domain.Library.Settings> allSettings = cacheManager.Get(SettingsAllKey, () => settings.ToList());

        return Task.FromResult(Result.Success(allSettings));
    }
}

