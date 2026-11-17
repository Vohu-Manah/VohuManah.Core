using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.GetCountByLanguage;

internal sealed class GetPublicationCountByLanguageQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetPublicationCountByLanguageQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByLanguageQuery query, CancellationToken cancellationToken)
    {
        var publications = dbContext.Publications;

        var list = await publications
            .Include(p => p.Language)
            .GroupBy(p => new { p.LanguageId, LanguageName = p.Language != null ? p.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

