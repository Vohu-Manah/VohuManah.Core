using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Attachments.Update;

internal sealed class UpdateAttachmentCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<UpdateAttachmentCommand>
{
    public async Task<Result> Handle(UpdateAttachmentCommand command, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = dbContext.Attachments;

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

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

