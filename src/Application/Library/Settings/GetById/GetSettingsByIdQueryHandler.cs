using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetById;

internal sealed class GetSettingsByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetSettingsByIdQuery, SettingsResponse>
{
    public async Task<Result<SettingsResponse>> Handle(GetSettingsByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        
        Domain.Library.Settings? setting = await settings
            .SingleOrDefaultAsync(s => s.Id == query.Id, cancellationToken);

        if (setting == null)
        {
            return Result.Failure<SettingsResponse>(SettingsErrors.NotFound(query.Id));
        }

        SettingsResponse response = new SettingsResponse {
            Id = setting.Id,
            Name = setting.Name,
            Value = setting.Value
        };

        return Result.Success(response);
    }
}

