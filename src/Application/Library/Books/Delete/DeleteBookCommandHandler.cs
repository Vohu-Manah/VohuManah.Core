using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.Delete;

internal sealed class DeleteBookCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        DbSet<Book> books = unitOfWork.Set<Book>();
        
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
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

