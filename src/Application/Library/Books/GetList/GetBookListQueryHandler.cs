using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetList;

internal sealed class GetBookListQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookListQuery, PagedList<BookListResponse>>
{
    public async Task<Result<PagedList<BookListResponse>>> Handle(GetBookListQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Book> books = unitOfWork.GetDbSet<Book>()
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Year))
        {
            books = books.Where(b => b.PublishYear == query.Year);
        }

        
        if (!string.IsNullOrWhiteSpace(query.FromDate))
        {
            string fromYear = query.FromDate.Length >= 4 ? query.FromDate[..4] : query.FromDate;
            books = books.Where(b => b.PublishYear.CompareTo(fromYear) >= 0);
        }

        if (!string.IsNullOrWhiteSpace(query.ToDate))
        {
            string toYear = query.ToDate.Length >= 4 ? query.ToDate[..4] : query.ToDate;
            books = books.Where(b => b.PublishYear.CompareTo(toYear) <= 0);
        }

        
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string term = query.Search.Trim();
            books = books.Where(b =>
                b.Name.Contains(term) ||
                b.No.Contains(term) ||
                b.Author.Contains(term) ||
                b.Isbn.Contains(term) ||
                (b.Publisher != null && b.Publisher.Name.Contains(term)) ||
                (b.Subject != null && b.Subject.Title.Contains(term)));
        }

        int totalCount = await books.CountAsync(cancellationToken);

        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 200);

        List<BookListResponse> items = await books
            .OrderByDescending(b => b.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
                BookShelfRow = b.BookShelfRow,
                PublishYear = b.PublishYear
            })
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedList<BookListResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }
}
