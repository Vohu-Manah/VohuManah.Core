using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Languages.Create;

internal sealed class CreateLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<CreateLanguageCommand, int>
{
    private const string LanguagePatternKey = "sdp.language.";

    public async Task<Result<int>> Handle(CreateLanguageCommand command, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.Set<Language>();
        
        if (await languages.AnyAsync(l => l.Name == command.Name, cancellationToken))
        {
            return Result.Failure<int>(Error.Conflict("Languages.NameNotUnique", "A language with this name already exists"));
        }

        Language language = new Language { 
            Name = command.Name,
            Abbreviation = command.Abbreviation
        };
        languages.Add(language);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(LanguagePatternKey);

        return language.Id;
    }
}


