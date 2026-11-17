using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetAll;

internal sealed class GetAllSettingsQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetAllSettingsQuery, List<SettingsResponse>>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<List<SettingsResponse>>> Handle(GetAllSettingsQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = dbContext.Settings;

        List<VohuManah.Domain.AppSettings.Settings> allSettings = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());

        List<SettingsResponse> response = allSettings.Select(s => new SettingsResponse
        {
            Id = s.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

