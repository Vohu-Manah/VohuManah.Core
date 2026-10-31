using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetBarcodeData;

internal sealed class GetBookBarcodeQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookBarcodeQuery, List<BookBarcodeResponse>>
{
    public async Task<Result<List<BookBarcodeResponse>>> Handle(
        GetBookBarcodeQuery query,
        CancellationToken cancellationToken)
    {
        
        
        
        long afterId = Math.Max(0, query.AfterId);
        int take = Math.Clamp(query.Take, 1, 5000);

        IQueryable<Book> books = unitOfWork.GetDbSet<Book>()
            .AsNoTracking()
            .Where(b => b.Id > afterId);

        
        if (!string.IsNullOrWhiteSpace(query.Year))
        {
            books = books.Where(b => b.PublishYear == query.Year);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            books = books.Where(b =>
                b.Name.Contains(term) ||
                b.No.Contains(term) ||
                b.Author.Contains(term) ||
                b.Isbn.Contains(term));
        }

        List<BookBarcodeResponse> items = await books
            .OrderBy(b => b.Id)
            .Take(take)
            .Select(b => new BookBarcodeResponse
            {
                Id = b.Id,
                Name = b.Name,
                No = b.No,
            })
            .ToListAsync(cancellationToken);

        return Result.Success(items);
    }
}
