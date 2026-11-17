using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.FileStorage;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Attachments.GetByEntity;
using VohuManah.Domain.Attachments;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetProfile;

internal sealed class GetBookProfileQueryHandler(
    IApplicationDbContext dbContext,
    IFileStorageService fileStorageService) : IQueryHandler<GetBookProfileQuery, BookProfileResponse>
{
    public async Task<Result<BookProfileResponse>> Handle(GetBookProfileQuery query, CancellationToken cancellationToken)
    {
        DbSet<Book> books = dbContext.Books;
        DbSet<Attachment> attachments = dbContext.Attachments;

        Book? book = await books
            .Include(b => b.Publisher)
            .Include(b => b.Subject)
            .Include(b => b.Language)
            .Include(b => b.City)
            .SingleOrDefaultAsync(b => b.Id == query.Id, cancellationToken);

        if (book == null)
        {
            return Result.Failure<BookProfileResponse>(BookErrors.NotFound(query.Id));
        }

        List<Attachment> bookAttachments = await attachments
            .Where(a => a.EntityType == "Book" && a.EntityId == query.Id)
            .OrderByDescending(a => a.IsMain)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        static bool IsImage(Attachment a)
        {
            var ct = a.ContentType?.ToUpperInvariant() ?? "";
            var name = (a.OriginalFileName ?? a.FileName ?? "").ToUpperInvariant();
            return ct.Contains("image", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(".webp", StringComparison.OrdinalIgnoreCase);
        }

        AttachmentResponse Map(Attachment a) => new AttachmentResponse
        {
            Id = a.Id,
            FileName = a.FileName,
            OriginalFileName = a.OriginalFileName,
            FileUrl = fileStorageService.GetFileUrl(a.FilePath),
            ContentType = a.ContentType,
            FileSize = a.FileSize,
            Description = a.Description,
            IsMain = a.IsMain,
            CreatedAt = a.CreatedAt
        };

        var mainCandidate = bookAttachments.FirstOrDefault(a => a.IsMain)
            ?? bookAttachments.FirstOrDefault(IsImage);

        AttachmentResponse? mainAttachment = mainCandidate is null ? null : Map(mainCandidate);

        List<AttachmentResponse> otherAttachments = bookAttachments
            .Where(a => a.Id != mainCandidate?.Id)
            .Select(Map)
            .ToList();

        BookProfileResponse response = new BookProfileResponse
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
            BookShelfRow = book.BookShelfRow,
            MainAttachment = mainAttachment,
            OtherAttachments = otherAttachments
        };

        return Result.Success(response);
    }
}

