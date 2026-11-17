using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.Delete;

internal sealed class DeleteBookCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        DbSet<Book> books = dbContext.Books;

        Book? book = await books
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .Include(b => b.Language)
            .Include(b => b.City)
            .SingleOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

        if (book == null)
        {
            return Result.Failure(BookErrors.NotFound(command.Id));
        }

        books.Remove(book);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

