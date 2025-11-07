using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.Create;

internal sealed class CreateBookCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBookCommand, long>
{
    public async Task<Result<long>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
        string name = command.Name.Trim();
        if (await books.AnyAsync(b => b.Name.Trim() == name, cancellationToken))
        {
            return Result.Failure<long>(BookErrors.NameNotUnique);
        }

        Book book = new Book
        {
            Name = command.Name,
            Author = command.Author,
            Translator = command.Translator,
            Editor = command.Editor,
            Corrector = command.Corrector,
            PublisherId = command.PublisherId,
            PublishPlaceId = command.PublishPlaceId,
            PublishYear = command.PublishYear,
            PublishOrder = command.PublishOrder,
            Isbn = command.Isbn,
            No = command.No,
            VolumeCount = command.VolumeCount,
            LanguageId = command.LanguageId,
            SubjectId = command.SubjectId,
            BookShelfRow = command.BookShelfRow
        };

        books.Add(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}

