using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Languages.GetAll;

internal sealed class GetAllLanguagesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllLanguagesQuery, List<LanguageResponse>>
{
    private const string LanguageAllKey = "sdp.language.all";

    public Task<Result<List<LanguageResponse>>> Handle(GetAllLanguagesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.GetDbSet<Language>();

        List<Language> allLanguages = cacheManager.Retrieve(LanguageAllKey, () => languages.ToList());

        List<LanguageResponse> response = allLanguages.Select(l => new LanguageResponse
        {
            Id = l.Id,
            Name = l.Name,
            Abbreviation = l.Abbreviation
        }).ToList();

        if (query.AddAllItemInFirstRow)
        {
            response.Insert(0, new LanguageResponse { Id = 0, Name = "همه زبانها", Abbreviation = "" });
        }

        return Task.FromResult(Result.Success(response));
    }
}


