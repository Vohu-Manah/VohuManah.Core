using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetCountByLanguage;

internal sealed class GetManuscriptCountByLanguageQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetManuscriptCountByLanguageQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetManuscriptCountByLanguageQuery query, CancellationToken cancellationToken)
    {
        var manuscripts = dbContext.Manuscripts;

        var list = await manuscripts
            .Include(m => m.Language)
            .GroupBy(m => new { m.LanguageId, LanguageName = m.Language != null ? m.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

