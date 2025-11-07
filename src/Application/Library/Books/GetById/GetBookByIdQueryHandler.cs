using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetById;

internal sealed class GetBookByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookByIdQuery, BookResponse>
{
    public async Task<Result<BookResponse>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
        Book? book = await books
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .Include(b => b.Language)
            .Include(b => b.City)
            .SingleOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (book == null)
        {
            return Result.Failure<BookResponse>(BookErrors.NotFound(query.Id));
        }

        BookResponse response = new BookResponse
        {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Translator = book.Translator,
            Editor = book.Editor,
            Corrector = book.Corrector,
            PublisherId = book.PublisherId,
            PublisherName = book.Publisher?.Name,
            PublishPlaceId = book.PublishPlaceId,
            PublishPlaceName = book.City?.Name,
            PublishYear = book.PublishYear,
            PublishOrder = book.PublishOrder,
            Isbn = book.Isbn,
            No = book.No,
            VolumeCount = book.VolumeCount,
            LanguageId = book.LanguageId,
            LanguageName = book.Language?.Name,
            SubjectId = book.SubjectId,
            SubjectTitle = book.Subject?.Title,
            BookShelfRow = book.BookShelfRow
        };

        return Result.Success(response);
    }
}

