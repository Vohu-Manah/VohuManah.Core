using Application.Abstractions.Data;
using Application.Abstractions.FileStorage;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.UpdateUserSettings;

internal sealed class UpdateUserSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : ICommandHandler<UpdateUserSettingsCommand>
{
    public async Task<Result> Handle(UpdateUserSettingsCommand command, CancellationToken cancellationToken)
    {
        var userSettings = unitOfWork.GetDbSet<UserSettings>();
        var attachments = unitOfWork.GetDbSet<Attachment>();

        var settings = await userSettings
            .Include(us => us.ProfileImageAttachment)
            .FirstOrDefaultAsync(us => us.UserId == command.UserId, cancellationToken);

        if (settings == null)
        {
            settings = new UserSettings
            {
                UserId = command.UserId,
            };
            userSettings.Add(settings);
        }

        if (command.ProfileImageAttachmentId.HasValue)
        {
            if (settings.ProfileImageAttachmentId.HasValue && settings.ProfileImageAttachment != null)
            {
                var oldAttachment = settings.ProfileImageAttachment;
                await fileStorageService.DeleteFileAsync(oldAttachment.FilePath, cancellationToken);
                attachments.Remove(oldAttachment);
            }

            settings.ProfileImageAttachmentId = command.ProfileImageAttachmentId;
        }

        if (command.ThemePrimaryColor != null)
        {
            settings.ThemePrimaryColor = command.ThemePrimaryColor;
        }

        if (command.ThemeSecondaryColor != null)
        {
            settings.ThemeSecondaryColor = command.ThemeSecondaryColor;
        }

        if (command.ThemeAccentColor != null)
        {
            settings.ThemeAccentColor = command.ThemeAccentColor;
        }

        if (command.ThemeMode != null)
        {
            settings.ThemeMode = command.ThemeMode;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

