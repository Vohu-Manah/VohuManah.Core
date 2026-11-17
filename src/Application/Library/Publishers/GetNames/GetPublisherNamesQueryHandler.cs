using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.GetNames;

internal sealed class GetPublisherNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublisherNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetPublisherNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.GetDbSet<Publisher>();

        List<SelectItemResponse> items = publishers
            .OrderBy(p => p.Name)
            .Select(p => new SelectItemResponse { Id = p.Id, Title = p.Name })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه ناشرین" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


