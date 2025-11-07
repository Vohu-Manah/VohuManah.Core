using Application.Library._Shared;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publishers.GetNames;

internal sealed class GetPublisherNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublisherNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetPublisherNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.Set<Publisher>();

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


