using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.PublicationTypes.GetById;

internal sealed class GetPublicationTypeByIdQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetPublicationTypeByIdQuery, PublicationTypeResponse>
{
    public async Task<Result<PublicationTypeResponse>> Handle(GetPublicationTypeByIdQuery query, CancellationToken cancellationToken)
    {
        DbSet<PublicationType> publicationTypes = unitOfWork.GetDbSet<PublicationType>();

        PublicationType? publicationType = await publicationTypes
            .SingleOrDefaultAsync(pt => pt.Id == query.Id, cancellationToken);

        if (publicationType == null)
        {
            return Result.Failure<PublicationTypeResponse>(PublicationTypeErrors.NotFound(query.Id));
        }

        PublicationTypeResponse response = new PublicationTypeResponse
        {
            Id = publicationType.Id,
            Title = publicationType.Title
        };

        return Result.Success(response);
    }
}

