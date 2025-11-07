using Application.Library._Shared;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Languages.GetNames;

internal sealed class GetLanguageNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetLanguageNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetLanguageNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Language> languages = unitOfWork.Set<Language>();

        List<SelectItemResponse> items = languages
            .OrderBy(l => l.Name)
            .Select(l => new SelectItemResponse { Id = l.Id, Title = l.Name })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه زبان ها" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


