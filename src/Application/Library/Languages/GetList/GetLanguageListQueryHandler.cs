using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.GetList;

internal sealed class GetLanguageListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetLanguageListQuery, List<LanguageListResponse>>
{
    private const string LanguageAllKey = "sdp.language.all";

    public Task<Result<List<LanguageListResponse>>> Handle(GetLanguageListQuery query, CancellationToken cancellationToken)
    {
        var languages = unitOfWork.GetDbSet<Language>();

        var allLanguages = cacheManager.Retrieve(LanguageAllKey, () => languages.ToList());

        var list = allLanguages.Select(l => new LanguageListResponse
        {
            Id = l.Id,
            Name = l.Name,
            Abbreviation = l.Abbreviation
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

