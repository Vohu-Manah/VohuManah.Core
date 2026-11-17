using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.AppSettings;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetById;

internal sealed class GetSettingsByIdQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetSettingsByIdQuery, SettingsResponse>
{
    public async Task<Result<SettingsResponse>> Handle(GetSettingsByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = dbContext.Settings;

        VohuManah.Domain.AppSettings.Settings? setting = await settings
            .SingleOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (setting == null)
        {
            return Result.Failure<SettingsResponse>(SettingsErrors.NotFound(query.Id));
        }

        SettingsResponse response = new SettingsResponse
        {
            Id = setting.Id,
            Name = setting.Name,
            Value = setting.Value
        };

        return Result.Success(response);
    }
}

