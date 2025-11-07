using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetBackground;

internal sealed class GetCurrentBackgroundImageNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetCurrentBackgroundImageNameQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetCurrentBackgroundImageNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        List<Domain.Library.Settings> all = cacheManager.Get(SettingsAllKey, () => settings.ToList());
        string name = all.Where(s => s.Name == "CurrentBackgroundImageName").Select(s => s.Value).SingleOrDefault() ?? string.Empty;
        return Task.FromResult(Result.Success(name));
    }
}


