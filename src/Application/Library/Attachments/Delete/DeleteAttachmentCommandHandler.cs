using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.FileStorage;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Attachments.Delete;

internal sealed class DeleteAttachmentCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : ICommandHandler<DeleteAttachmentCommand>
{
    public async Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = unitOfWork.GetDbSet<Attachment>();

        Attachment? attachment = await attachments
            .FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure(AttachmentErrors.NotFound(command.Id));
        }

        try
        {
            await fileStorageService.DeleteFileAsync(attachment.FilePath, cancellationToken);

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


