using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetCount;

internal sealed class GetBookCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountQuery, int>
{
    public async Task<Result<int>> Handle(GetBookCountQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        int count = await books.CountAsync(cancellationToken);

        return Result.Success(count);
    }
}

