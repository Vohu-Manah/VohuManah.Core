using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Languages;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.GetNames;

internal sealed class GetLanguageNamesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetLanguageNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetLanguageNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = dbContext.Languages;

        List<SelectItemResponse> items = languages
            .OrderBy(l => l.Name)
            .Select(l => new SelectItemResponse { Id = l.Id, Title = l.Name })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه زبان ها" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


