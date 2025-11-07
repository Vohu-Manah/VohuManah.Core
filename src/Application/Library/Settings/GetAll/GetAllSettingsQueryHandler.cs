using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetAll;

internal sealed class GetAllSettingsQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllSettingsQuery, List<SettingsResponse>>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<List<SettingsResponse>>> Handle(GetAllSettingsQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        
        List<Domain.Library.Settings> allSettings = cacheManager.Get(SettingsAllKey, () => settings.ToList());
        
        List<SettingsResponse> response = allSettings.Select(s => new SettingsResponse
        {
            Id = s.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}

