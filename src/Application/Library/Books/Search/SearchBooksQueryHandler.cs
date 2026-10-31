using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.Search;

internal sealed class SearchBooksQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<SearchBooksQuery, PagedResult<BookSearchResponse>>
{
    public async Task<Result<PagedResult<BookSearchResponse>>> Handle(SearchBooksQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.GetDbSet<Book>();

        IQueryable<Book> queryable = books.AsQueryable();

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryable = queryable.Where(b => b.Name.Contains(query.Name));
        }

        if (!string.IsNullOrEmpty(query.Author))
        {
            queryable = queryable.Where(b => b.Author.Contains(query.Author));
        }

        if (!string.IsNullOrEmpty(query.Translator))
        {
            queryable = queryable.Where(b => b.Translator != null && b.Translator.Contains(query.Translator));
        }

        if (!string.IsNullOrEmpty(query.Editor))
        {
            queryable = queryable.Where(b => b.Editor != null && b.Editor.Contains(query.Editor));
        }

        if (!string.IsNullOrEmpty(query.Isbn))
        {
            queryable = queryable.Where(b => b.Isbn == query.Isbn);
        }

        if (!string.IsNullOrEmpty(query.No))
        {
            queryable = queryable.Where(b => b.No == query.No);
        }

        if (!string.IsNullOrEmpty(query.FromPublishYear))
        {
            queryable = queryable.Where(b => b.PublishYear.CompareTo(query.FromPublishYear) >= 0);
        }

        if (!string.IsNullOrEmpty(query.ToPublishYear))
        {
            queryable = queryable.Where(b => b.PublishYear.CompareTo(query.ToPublishYear) <= 0);
        }

        if (query.FromVolumeCount != 0)
        {
            queryable = queryable.Where(b => b.VolumeCount >= query.FromVolumeCount);
        }

        if (query.ToVolumeCount != 0)
        {
            queryable = queryable.Where(b => b.VolumeCount <= query.ToVolumeCount);
        }

        if (query.LanguageId != 0)
        {
            queryable = queryable.Where(b => b.LanguageId == query.LanguageId);
        }

        if (query.SubjectId != 0)
        {
            queryable = queryable.Where(b => b.SubjectId == query.SubjectId);
        }

        if (query.PublishPlaceId != 0)
        {
            queryable = queryable.Where(b => b.PublishPlaceId == query.PublishPlaceId);
        }

        if (query.PublisherId != 0)
        {
            queryable = queryable.Where(b => b.PublisherId == query.PublisherId);
        }

        
        
        queryable = queryable.OrderBy(b => b.Name).ThenBy(b => b.Id);

        
        
        int page = Math.Max(1, query.Page);
        int pageSize = Math.Clamp(query.PageSize, 1, 50000);

        
        
        
        var rows = await queryable
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new
            {
                TotalCount = 0L,
                b.Id,
                b.Name,
                b.No,
                b.Author,
                b.Editor,
                b.Translator,
                b.Isbn,
                Subject = b.Subject != null ? b.Subject.Title : string.Empty,
                PublishPlaceName = b.City != null ? b.City.Name : string.Empty,
                b.VolumeCount,
                b.PublishYear,
                Language = b.Language != null ? b.Language.Name : string.Empty,
                b.PublishOrder,
                PublisherName = b.Publisher != null ? b.Publisher.Name : string.Empty,
                b.BookShelfRow,
                b.Corrector,
            })
            .ToListAsync(cancellationToken);

        
        long totalCount = await queryable.LongCountAsync(cancellationToken);

        List<BookSearchResponse> items = rows
            .Select(r => new BookSearchResponse
            {
                Id = r.Id,
                Name = r.Name,
                No = r.No,
                Author = r.Author,
                Editor = r.Editor ?? string.Empty,
                Translator = r.Translator ?? string.Empty,
                Isbn = r.Isbn,
                Subject = r.Subject,
                PublishPlaceName = r.PublishPlaceName,
                VolumeCount = r.VolumeCount,
                PublishYear = r.PublishYear,
                Language = r.Language,
                PublishOrder = r.PublishOrder,
                PublisherName = r.PublisherName,
                BookShelfRow = r.BookShelfRow,
                Corrector = r.Corrector ?? string.Empty,
            })
            .ToList();

        return Result.Success(new PagedResult<BookSearchResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
        });
    }
}
