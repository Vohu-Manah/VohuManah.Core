using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetBackground;

internal sealed class GetCurrentBackgroundImageFolderQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetCurrentBackgroundImageFolderQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetCurrentBackgroundImageFolderQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.AppSettings.Settings>();
        List<VohuManah.Domain.AppSettings.Settings> all = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());
        string folder = all.Where(s => s.Name == "BackgroundImageFolder").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        return Task.FromResult(Result.Success(folder));
    }
}
