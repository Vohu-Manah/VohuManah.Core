using Application.Abstractions.Data;
using Application.Abstractions.FileStorage;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.GetUserSettings;

internal sealed class GetUserSettingsQueryHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : IQueryHandler<GetUserSettingsQuery, UserSettingsResponse>
{
    public async Task<Result<UserSettingsResponse>> Handle(GetUserSettingsQuery query, CancellationToken cancellationToken)
    {
        var userSettings = unitOfWork.GetDbSet<UserSettings>();

        var settings = await userSettings
            .Include(us => us.ProfileImageAttachment)
            .FirstOrDefaultAsync(us => us.UserId == query.UserId, cancellationToken);

        if (settings == null)
        {
            var newSettings = new UserSettings
            {
                UserId = query.UserId,
            };
            userSettings.Add(newSettings);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new UserSettingsResponse
            {
                Id = newSettings.Id,
                UserId = newSettings.UserId,
                ProfileImageAttachmentId = newSettings.ProfileImageAttachmentId,
                ProfileImageUrl = null,
                ThemePrimaryColor = newSettings.ThemePrimaryColor,
                ThemeSecondaryColor = newSettings.ThemeSecondaryColor,
                ThemeAccentColor = newSettings.ThemeAccentColor,
                ThemeMode = newSettings.ThemeMode,
            });
        }

        string? profileImageUrl = null;
        if (settings.ProfileImageAttachmentId.HasValue && settings.ProfileImageAttachment != null)
        {
            profileImageUrl = fileStorageService.GetFileUrl(settings.ProfileImageAttachment.FilePath);
        }

        return Result.Success(new UserSettingsResponse
        {
            Id = settings.Id,
            UserId = settings.UserId,
            ProfileImageAttachmentId = settings.ProfileImageAttachmentId,
            ProfileImageUrl = profileImageUrl,
            ThemePrimaryColor = settings.ThemePrimaryColor,
            ThemeSecondaryColor = settings.ThemeSecondaryColor,
            ThemeAccentColor = settings.ThemeAccentColor,
            ThemeMode = settings.ThemeMode,
        });
    }
}

