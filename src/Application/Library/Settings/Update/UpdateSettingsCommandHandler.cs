using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Settings.Update;

internal sealed class UpdateSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateSettingsCommand>
{
    private const string SettingsPatternKey = "sdp.settings.";

    public async Task<Result> Handle(UpdateSettingsCommand command, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Library.Settings> settings = unitOfWork.GetDbSet<VohuManah.Domain.Library.Settings>();

        VohuManah.Domain.Library.Settings? existing = await settings
            .SingleOrDefaultAsync(s => s.Name.Trim() == command.Name.Trim(), cancellationToken);

        if (existing == null)
        {
            return Result.Failure(SettingsErrors.NotFound(command.Name));
        }

        existing.Value = command.Value;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(SettingsPatternKey);

        return Result.Success();
    }
}

