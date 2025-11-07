using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Publications.GetCountByType;

internal sealed class GetPublicationCountByTypeQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationCountByTypeQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetPublicationCountByTypeQuery query, CancellationToken cancellationToken)
    {
        var publications = unitOfWork.Set<Publication>();
        
        var list = await publications
            .Include(p => p.PublicationType)
            .GroupBy(p => new { p.TypeId, TypeTitle = p.PublicationType != null ? p.PublicationType.Title : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.TypeTitle,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

