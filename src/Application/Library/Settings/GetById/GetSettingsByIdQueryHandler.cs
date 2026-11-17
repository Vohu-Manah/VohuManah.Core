using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetById;

internal sealed class GetSettingsByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetSettingsByIdQuery, SettingsResponse>
{
    public async Task<Result<SettingsResponse>> Handle(GetSettingsByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Library.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.Library.Settings>();

        VohuManah.Domain.Library.Settings? setting = await settings
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

