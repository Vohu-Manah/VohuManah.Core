using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.Update;

internal sealed class UpdateBookCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateBookCommand>
{
    public async Task<Result> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.GetDbSet<Book>();

        Book? existing = await books
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .Include(b => b.Language)
            .Include(b => b.City)
            .SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (existing == null)
        {
            return Result.Failure(BookErrors.NotFound(command.Id));
        }

        
        
        
        

        existing.Name = command.Name;
        existing.Author = command.Author;
        existing.Editor = command.Editor;
        existing.Isbn = command.Isbn;
        existing.LanguageId = command.LanguageId;
        existing.No = command.No;
        existing.PublishOrder = command.PublishOrder;
        existing.PublishPlaceId = command.PublishPlaceId;
        existing.PublishYear = command.PublishYear;
        existing.PublisherId = command.PublisherId;
        existing.SubjectId = command.SubjectId;
        existing.Translator = command.Translator;
        existing.VolumeCount = command.VolumeCount;
        existing.Corrector = command.Corrector;
        
        
        if (!string.IsNullOrWhiteSpace(command.BookShelfRow))
        {
            existing.BookShelfRow = command.BookShelfRow;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

