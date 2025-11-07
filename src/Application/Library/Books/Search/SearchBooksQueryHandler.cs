using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.Search;

internal sealed class SearchBooksQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<SearchBooksQuery, List<BookSearchResponse>>
{
    public async Task<Result<List<BookSearchResponse>>> Handle(SearchBooksQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
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
            queryable = queryable.Where(b => string.Compare(b.PublishYear, query.FromPublishYear, StringComparison.Ordinal) >= 0);
        }

        if (!string.IsNullOrEmpty(query.ToPublishYear))
        {
            queryable = queryable.Where(b => string.Compare(b.PublishYear, query.ToPublishYear, StringComparison.Ordinal) <= 0);
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

        if (!string.IsNullOrEmpty(query.Corrector))
        {
            queryable = queryable.Where(b => b.Corrector != null && b.Corrector.Contains(query.Corrector));
        }

        queryable = queryable.OrderBy(b => b.Name);

        List<BookSearchResponse> results = await queryable
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .Include(b => b.Language)
            .Include(b => b.City)
            .Select(b => new BookSearchResponse
            {
                Name = b.Name,
                No = b.No,
                Author = b.Author,
                Editor = b.Editor ?? string.Empty,
                Translator = b.Translator ?? string.Empty,
                Isbn = b.Isbn,
                Subject = b.Subject != null ? b.Subject.Title : string.Empty,
                PublishPlaceName = b.City != null ? b.City.Name : string.Empty,
                VolumeCount = b.VolumeCount,
                PublishYear = b.PublishYear,
                Language = b.Language != null ? b.Language.Name : string.Empty,
                PublishOrder = b.PublishOrder,
                PublisherName = b.Publisher != null ? b.Publisher.Name : string.Empty,
                BookShelfRow = b.BookShelfRow,
                Corrector = b.Corrector ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return Result.Success(results);
    }
}
