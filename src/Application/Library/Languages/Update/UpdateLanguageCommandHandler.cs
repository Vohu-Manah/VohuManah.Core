using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Languages;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.Update;

internal sealed class UpdateLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateLanguageCommand>
{
    private const string LanguagePatternKey = "sdp.language.";

    public async Task<Result> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.GetDbSet<Language>();

        Language? existing = await languages.SingleOrDefaultAsync(l => l.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Languages.NotFound", $"Language with Id {command.Id} not found"));
        }

        if (await languages.AnyAsync(l => l.Id != command.Id && l.Name == command.Name, cancellationToken))
        {
            return Result.Failure(LanguageErrors.NameNotUnique);
        }

        existing.Name = command.Name;
        existing.Abbreviation = command.Abbreviation;
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(LanguagePatternKey);

        return Result.Success();
    }
}


