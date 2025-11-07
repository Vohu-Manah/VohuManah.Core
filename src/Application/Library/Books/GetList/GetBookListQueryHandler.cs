using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetList;

internal sealed class GetBookListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookListQuery, List<BookListResponse>>
{
    public async Task<Result<List<BookListResponse>>> Handle(GetBookListQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        var list = await books
            .Include(b => b.Publisher)
            .Select(b => new BookListResponse
            {
                Id = b.Id,
                Name = b.Name,
                Author = b.Author,
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                Translator = b.Translator,
                Corrector = b.Corrector,
                No = b.No,
                Isbn = b.Isbn,
                VolumeCount = b.VolumeCount,
                BookShelfRow = b.BookShelfRow
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

