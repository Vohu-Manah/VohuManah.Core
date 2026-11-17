using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetBackground;

internal sealed class GetCurrentBackgroundImageNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetCurrentBackgroundImageNameQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetCurrentBackgroundImageNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Library.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.Library.Settings>();
        List<VohuManah.Domain.Library.Settings> all = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());
        string name = all.Where(s => s.Name == "CurrentBackgroundImageName").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        return Task.FromResult(Result.Success(name));
    }
}


