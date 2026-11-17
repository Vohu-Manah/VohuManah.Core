using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.AppSettings;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetByName;

internal sealed class GetSettingsByNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetSettingsByNameQuery, SettingsResponse>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<SettingsResponse>> Handle(GetSettingsByNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.AppSettings.Settings>();

        List<VohuManah.Domain.AppSettings.Settings> allSettings = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());

        VohuManah.Domain.AppSettings.Settings? setting = allSettings.SingleOrDefault(s => s.Name.Trim() == query.Name.Trim());

        if (setting == null)
        {
            return Task.FromResult(Result.Failure<SettingsResponse>(SettingsErrors.NotFound(query.Name)));
        }

        SettingsResponse response = new SettingsResponse
        {
            Id = setting.Id,
            Name = setting.Name,
            Value = setting.Value
        };

        return Task.FromResult(Result.Success(response));
    }
}

