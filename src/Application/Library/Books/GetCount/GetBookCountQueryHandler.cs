using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCount;

internal sealed class GetBookCountQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetBookCountQuery, int>
{
    public async Task<Result<int>> Handle(GetBookCountQuery query, CancellationToken cancellationToken)
    {
        var books = dbContext.Books;

        int count = await books.CountAsync(cancellationToken);

        return Result.Success(count);
    }
}

