using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetByName;

internal sealed class GetSettingsByNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetSettingsByNameQuery, SettingsResponse>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<SettingsResponse>> Handle(GetSettingsByNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        
        List<Domain.Library.Settings> allSettings = cacheManager.Get(SettingsAllKey, () => settings.ToList());
        
        Domain.Library.Settings? setting = allSettings.SingleOrDefault(s => s.Name.Trim() == query.Name.Trim());

        if (setting == null)
        {
            return Task.FromResult(Result.Failure<SettingsResponse>(SettingsErrors.NotFound(query.Name)));
        }

        SettingsResponse response = new SettingsResponse {
            Id = setting.Id,
            Name = setting.Name,
            Value = setting.Value
        };

        return Task.FromResult(Result.Success(response));
    }
}

