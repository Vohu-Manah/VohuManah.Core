using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Domain.PublicationTypes;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.GetNames;

internal sealed class GetPublicationTypeNamesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationTypeNamesQuery, List<SelectItemResponse>>
{
    public Task<Result<List<SelectItemResponse>>> Handle(GetPublicationTypeNamesQuery query, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> types = unitOfWork.GetDbSet<PublicationType>();

        List<SelectItemResponse> items = types
            .OrderBy(t => t.Title)
            .Select(t => new SelectItemResponse { Id = t.Id, Title = t.Title })
            .ToList();

        if (query.AddAllItemInFirstRow)
        {
            items.Insert(0, new SelectItemResponse { Id = 0, Title = "همه انواع نشریه" });
        }

        return Task.FromResult(Result.Success(items));
    }
}


