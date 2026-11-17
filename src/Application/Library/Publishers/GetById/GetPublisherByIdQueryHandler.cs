using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publishers.GetById;

internal sealed class GetPublisherByIdQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetPublisherByIdQuery, PublisherResponse>
{
    public async Task<Result<PublisherResponse>> Handle(GetPublisherByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<Publisher> publishers = dbContext.Publishers;

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

