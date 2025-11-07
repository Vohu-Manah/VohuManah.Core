using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Languages.GetById;

internal sealed class GetLanguageByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetLanguageByIdQuery, LanguageResponse>
{
    public async Task<Result<LanguageResponse>> Handle(GetLanguageByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.Set<Language>();
        
        Language? language = await languages
            .SingleOrDefaultAsync(l => l.Id == query.Id, cancellationToken);

        if (language == null)
        {
            return Result.Failure<LanguageResponse>(LanguageErrors.NotFound(query.Id));
        }

        LanguageResponse response = new LanguageResponse {
            Id = language.Id,
            Name = language.Name,
            Abbreviation = language.Abbreviation
        };

        return Result.Success(response);
    }
}

