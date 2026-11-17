using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCountByLanguage;

internal sealed class GetBookCountByLanguageQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountByLanguageQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByLanguageQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.GetDbSet<Book>();

        var list = await books
            .Include(b => b.Language)
            .GroupBy(b => new { b.LanguageId, LanguageName = b.Language != null ? b.Language.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.LanguageName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

