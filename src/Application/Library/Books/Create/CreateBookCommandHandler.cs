using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.Create;

internal sealed class CreateBookCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateBookCommand, long>
{
    public async Task<Result<long>> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.GetDbSet<Book>();


        var book = new Book
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
            
            
            BookShelfRow = string.IsNullOrWhiteSpace(command.BookShelfRow) ? "-" : command.BookShelfRow
        };

        books.Add(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return book.Id;
    }
}

