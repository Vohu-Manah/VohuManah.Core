using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Attachments.Update;

internal sealed class UpdateAttachmentCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateAttachmentCommand>
{
    public async Task<Result> Handle(UpdateAttachmentCommand command, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = unitOfWork.GetDbSet<Attachment>();

        Attachment? attachment = await attachments
            .SingleOrDefaultAsync(a => a.Id == command.Id, cancellationToken);

        if (attachment == null)
        {
            return Result.Failure(AttachmentErrors.NotFound(command.Id));
        }

        if (command.Description != null)
        {
            attachment.Description = command.Description;
        }

        if (command.IsMain.HasValue)
        {
            if (command.IsMain.Value)
            {
                List<Attachment> existingMainAttachments = await attachments
                    .Where(a => a.EntityType == attachment.EntityType &&
                               a.EntityId == attachment.EntityId &&
                               a.Id != command.Id &&
                               a.IsMain)
                    .ToListAsync(cancellationToken);

                foreach (Attachment existingMain in existingMainAttachments)
                {
                    existingMain.IsMain = false;
                }
            }

            attachment.IsMain = command.IsMain.Value;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

