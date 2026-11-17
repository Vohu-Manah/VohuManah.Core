using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetAllEntities;

internal sealed class GetAllBookEntitiesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllBookEntitiesQuery, List<Book>>
{
    public async Task<Result<List<Book>>> Handle(GetAllBookEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.GetDbSet<Book>();

        List<Book> allBooks = await books.ToListAsync(cancellationToken);

        return Result.Success(allBooks);
    }
}

