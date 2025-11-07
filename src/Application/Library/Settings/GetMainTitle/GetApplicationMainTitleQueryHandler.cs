using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetMainTitle;

internal sealed class GetApplicationMainTitleQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetApplicationMainTitleQuery, string>
{
    private const string SettingsAllKey = "sdp.settings.all";

    public Task<Result<string>> Handle(GetApplicationMainTitleQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();

        List<Domain.Library.Settings> all = cacheManager.Get(SettingsAllKey, () => settings.ToList());
        string title = all.Where(s => s.Name == "ApplicationMainTitle").Select(s => s.Value).SingleOrDefault() ?? string.Empty;

        return Task.FromResult(Result.Success(title));
    }
}


