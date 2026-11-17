using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.Create;

internal sealed class CreateLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateLanguageCommand, int>
{
    private const string LanguagePatternKey = "sdp.language.";

    public async Task<Result<int>> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.GetDbSet<Language>();

        if (await languages.AnyAsync(l => l.Name == command.Name, cancellationToken))
        {
            return Result.Failure<int>(LanguageErrors.NameNotUnique);
        }

        var language = new Language
        {
            Name = command.Name,
            Abbreviation = command.Abbreviation
        };
        languages.Add(language);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        cacheManager.RemoveByPattern(LanguagePatternKey);

        return language.Id;
    }
}


