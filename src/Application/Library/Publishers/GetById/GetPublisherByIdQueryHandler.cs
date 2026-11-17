using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.GetById;

internal sealed class GetPublisherByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublisherByIdQuery, PublisherResponse>
{
    public async Task<Result<PublisherResponse>> Handle(GetPublisherByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = unitOfWork.GetDbSet<Publisher>();

        Publisher? publisher = await publishers
            .SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

        if (publisher == null)
        {
            return Result.Failure<PublisherResponse>(PublisherErrors.NotFound(query.Id));
        }

        PublisherResponse response = new PublisherResponse
        {
            Id = publisher.Id,
            Name = publisher.Name,
            ManagerName = publisher.ManagerName,
            PlaceId = publisher.PlaceId,
            Address = publisher.Address,
            Telephone = publisher.Telephone,
            Website = publisher.Website,
            Email = publisher.Email
        };

        return Result.Success(response);
    }
}

