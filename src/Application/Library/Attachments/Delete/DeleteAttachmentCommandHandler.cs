using Application.Abstractions.Data;
using Application.Abstractions.FileStorage;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Attachments.Delete;

internal sealed class DeleteAttachmentCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : ICommandHandler<DeleteAttachmentCommand>
{
    public async Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = unitOfWork.Set<Attachment>();

        Attachment? attachment = await attachments
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure(AttachmentErrors.NotFound(command.Id));
        }

        try
        {
            // Delete file from storage
            await fileStorageService.DeleteFileAsync(attachment.FilePath, cancellationToken);

            // Delete record from database
            attachments.Remove(attachment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(AttachmentErrors.DeleteFailed);
        }
    }
}


