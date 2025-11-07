using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Manuscripts.Create;

internal sealed class CreateManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateManuscriptCommand, int>
{
    public async Task<Result<int>> Handle(CreateManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.Set<Manuscript>();
        
        string name = command.Name.Trim();
        if (await manuscripts.AnyAsync(m => m.Name.Trim() == name, cancellationToken))
        {
            return Result.Failure<int>(ManuscriptErrors.NameNotUnique);
        }

        Manuscript manuscript = new Manuscript {
            Name = command.Name,
            Author = command.Author,
            WritingPlaceId = command.WritingPlaceId,
            Year = command.Year,
            PageCount = command.PageCount,
            Size = command.Size,
            GapId = command.GapId,
            VersionNo = command.VersionNo,
            LanguageId = command.LanguageId,
            SubjectId = command.SubjectId
        };

        manuscripts.Add(manuscript);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return manuscript.Id;
    }
}

