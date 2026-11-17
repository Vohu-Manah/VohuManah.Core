using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Manuscripts;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.GetAllEntities;

internal sealed class GetAllManuscriptEntitiesQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetAllManuscriptEntitiesQuery, List<Manuscript>>
{
    public async Task<Result<List<Manuscript>>> Handle(GetAllManuscriptEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>();

        List<Manuscript> allManuscripts = await manuscripts.ToListAsync(cancellationToken);

        return Result.Success(allManuscripts);
    }
}

