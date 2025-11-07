using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Books.GetVolumeCount;

internal sealed class GetBookVolumeCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookVolumeCountQuery, int>
{
    public async Task<Result<int>> Handle(GetBookVolumeCountQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.Set<Book>();
        
        int volumeCount = await books.SumAsync(b => (int?)b.VolumeCount, cancellationToken) ?? 0;

        return Result.Success(volumeCount);
    }
}

