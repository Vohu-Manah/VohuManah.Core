using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Languages.GetList;

internal sealed class GetLanguageListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetLanguageListQuery, List<LanguageListResponse>>
{
    private const string LanguageAllKey = "sdp.language.all";

    public Task<Result<List<LanguageListResponse>>> Handle(GetLanguageListQuery query, CancellationToken cancellationToken)
    {
        var languages = unitOfWork.Set<Language>();
        
        var allLanguages = cacheManager.Get(LanguageAllKey, () => languages.ToList());
        
        var list = allLanguages.Select(l => new LanguageListResponse
        {
            Id = l.Id,
            Name = l.Name,
            Abbreviation = l.Abbreviation
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

