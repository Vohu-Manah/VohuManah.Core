using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.Delete;

internal sealed class DeleteLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<DeleteLanguageCommand>
{
    private const string LanguagePatternKey = "sdp.language.";

    public async Task<Result> Handle(DeleteLanguageCommand command, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.GetDbSet<Language>();

        Language? existing = await languages.SingleOrDefaultAsync(l => l.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Languages.NotFound", $"Language with Id {command.Id} not found"));
        }

        languages.Remove(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(LanguagePatternKey);

        return Result.Success();
    }
}


