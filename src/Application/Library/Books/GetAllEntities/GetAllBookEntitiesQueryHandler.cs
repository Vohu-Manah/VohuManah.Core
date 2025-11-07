using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetAllEntities;

internal sealed class GetAllBookEntitiesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllBookEntitiesQuery, List<Book>>
{
    public async Task<Result<List<Book>>> Handle(GetAllBookEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
        List<Book> allBooks = await books.ToListAsync(cancellationToken);

        return Result.Success(allBooks);
    }
}

