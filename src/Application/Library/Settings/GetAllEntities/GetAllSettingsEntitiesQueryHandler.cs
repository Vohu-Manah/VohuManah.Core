using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetAllEntities;

internal sealed class GetAllSettingsEntitiesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllSettingsEntitiesQuery, List<VohuManah.Domain.AppSettings.Settings>>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<List<VohuManah.Domain.AppSettings.Settings>>> Handle(GetAllSettingsEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.AppSettings.Settings>();

        List<VohuManah.Domain.AppSettings.Settings> allSettings = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());

        return Task.FromResult(Result.Success(allSettings));
    }
}

