using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetBackground;

internal sealed class GetBackgroundImagePathQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetBackgroundImagePathQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetBackgroundImagePathQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        List<Domain.Library.Settings> all = cacheManager.Get(SettingsAllKey, () => settings.ToList());
        string folder = all.Where(s => s.Name == "BackgroundImageFolder").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        string name = all.Where(s => s.Name == "CurrentBackgroundImageName").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        string path = string.IsNullOrWhiteSpace(folder) || string.IsNullOrWhiteSpace(name) ? string.Empty : System.IO.Path.Combine(folder, name);
        return Task.FromResult(Result.Success(path));
    }
}


