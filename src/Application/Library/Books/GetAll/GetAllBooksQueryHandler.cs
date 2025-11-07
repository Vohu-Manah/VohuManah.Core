using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetAll;

internal sealed class GetAllBooksQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllBooksQuery, List<BookResponse>>
{
    public async Task<Result<List<BookResponse>>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
        List<BookResponse> allBooks = await books
            .Include(b => b.Publisher)
            .Select(b => new BookResponse
            {
                Id = b.Id,
                Name = b.Name,
                Author = b.Author,
                Translator = b.Translator,
                Editor = b.Editor,
                Corrector = b.Corrector,
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher != null ? b.Publisher.Name : null,
                PublishPlaceId = b.PublishPlaceId,
                PublishPlaceName = b.City != null ? b.City.Name : null,
                PublishYear = b.PublishYear,
                PublishOrder = b.PublishOrder,
                Isbn = b.Isbn,
                No = b.No,
                VolumeCount = b.VolumeCount,
                LanguageId = b.LanguageId,
                LanguageName = b.Language != null ? b.Language.Name : null,
                SubjectId = b.SubjectId,
                SubjectTitle = b.Subject != null ? b.Subject.Title : null,
                BookShelfRow = b.BookShelfRow
            })
            .ToListAsync(cancellationToken);

        return Result.Success(allBooks);
    }
}
