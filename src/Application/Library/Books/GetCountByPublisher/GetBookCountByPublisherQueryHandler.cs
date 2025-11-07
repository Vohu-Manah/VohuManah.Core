using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Library._Shared;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetCountByPublisher;

internal sealed class GetBookCountByPublisherQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountByPublisherQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByPublisherQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        var list = await books
            .Include(b => b.Publisher)
            .GroupBy(b => new { b.PublisherId, PublisherName = b.Publisher != null ? b.Publisher.Name : "" })
            .Select(g => new ListItemResponse
            {
                Text = g.Key.PublisherName,
                Value = g.Count()
            })
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

