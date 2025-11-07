using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Settings.Update;

internal sealed class UpdateSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateSettingsCommand>
{
    private const string SettingsPatternKey = "sdp.settings.";

    public async Task<Result> Handle(UpdateSettingsCommand command, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.Settings> settings = unitOfWork.Set<Domain.Library.Settings>();
        
        Domain.Library.Settings? existing = await settings
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

