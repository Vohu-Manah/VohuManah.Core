using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Books.GetVolumeCount;

internal sealed class GetBookVolumeCountQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetBookVolumeCountQuery, int>
{
    public async Task<Result<int>> Handle(GetBookVolumeCountQuery query, CancellationToken cancellationToken)
    {
        var books = unitOfWork.GetDbSet<Book>();

        int volumeCount = await books.SumAsync(b => (int?)b.VolumeCount, cancellationToken) ?? 0;

        return Result.Success(volumeCount);
    }
}

