using Application.Abstractions.Data;
using Application.Abstractions.FileStorage;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Attachments.Upload;

internal sealed class UploadAttachmentCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : ICommandHandler<UploadAttachmentCommand, long>
{
    public async Task<Result<long>> Handle(UploadAttachmentCommand command, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = unitOfWork.Set<Attachment>();

        try
        {
            // Save file to storage
            string savedFileName = await fileStorageService.SaveFileAsync(
                command.FileStream,
                command.FileName,
                command.ContentType,
                cancellationToken);

            // Create attachment record
            Attachment attachment = new Attachment
            {
                FileName = savedFileName,
                OriginalFileName = command.FileName,
                FilePath = savedFileName, // Relative path
                ContentType = command.ContentType,
                FileSize = command.FileSize,
                EntityType = command.EntityType,
                EntityId = command.EntityId,
                Description = command.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.CreatedBy
            };

            attachments.Add(attachment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return attachment.Id;
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<long>(Error.Validation("Attachments.Validation", ex.Message));
        }
        catch (Exception)
        {
            return Result.Failure<long>(AttachmentErrors.UploadFailed);
        }
    }
}


