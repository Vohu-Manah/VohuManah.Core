using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Languages.Update;

internal sealed class UpdateLanguageCommandHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : ICommandHandler<UpdateLanguageCommand>
{
    private const string LanguagePatternKey = "sdp.language.";

    public async Task<Result> Handle(UpdateLanguageCommand command, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.Set<Language>();
        
        Language? existing = await languages.SingleOrDefaultAsync(l => l.Id == command.Id, cancellationToken);
        if (existing == null)
        {
            return Result.Failure(Error.NotFound("Languages.NotFound", $"Language with Id {command.Id} not found"));
        }

        if (await languages.AnyAsync(l => l.Id != command.Id && l.Name == command.Name, cancellationToken))
        {
            return Result.Failure(Error.Conflict("Languages.NameNotUnique", "A language with this name already exists"));
        }

        existing.Name = command.Name;
        existing.Abbreviation = command.Abbreviation;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        cacheManager.RemoveByPattern(LanguagePatternKey);

        return Result.Success();
    }
}


