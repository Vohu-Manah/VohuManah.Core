using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.GetBackground;

internal sealed class GetBackgroundImagePathQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetBackgroundImagePathQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetBackgroundImagePathQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.AppSettings.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.AppSettings.Settings>();
        List<VohuManah.Domain.AppSettings.Settings> all = cacheManager.Retrieve(SettingsAllKey, () => settings.ToList());
        string folder = all.Where(s => s.Name == "BackgroundImageFolder").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        string name = all.Where(s => s.Name == "CurrentBackgroundImageName").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        string path = string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(name) ? string.Empty : System.IO.Path.Combine(folder, name);
        return Task.FromResult(Result.Success(path));
    }
}


