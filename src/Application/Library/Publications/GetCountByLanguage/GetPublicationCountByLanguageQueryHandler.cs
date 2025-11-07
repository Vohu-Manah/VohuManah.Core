using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetCountByLanguage;

internal sealed class GetPublicationCountByLanguageQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountByLanguageQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByLanguageQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
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

