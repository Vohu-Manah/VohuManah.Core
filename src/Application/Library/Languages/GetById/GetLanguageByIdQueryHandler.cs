using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Languages;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Languages.GetById;

internal sealed class GetLanguageByIdQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetLanguageByIdQuery, LanguageResponse>
{
    public async Task<Result<LanguageResponse>> Handle(GetLanguageByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = dbContext.Languages;

        Language? language = await languages
            .SingleOrDefaultAsync(l => l.Id == query.Id, cancellationToken);

        if (language == null)
        {
            return Result.Failure<LanguageResponse>(LanguageErrors.NotFound(query.Id));
        }

        LanguageResponse response = new LanguageResponse
        {
            Id = language.Id,
            Name = language.Name,
            Abbreviation = language.Abbreviation
        };

        return Result.Success(response);
    }
}

