using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.Books;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetCountByPublisher;

internal sealed class GetBookCountByPublisherQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookCountByPublisherQuery, List<ListItemResponse>>
{
    public async Task<Result<List<ListItemResponse>>> Handle(GetBookCountByPublisherQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.GetDbSet<Book>();

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

