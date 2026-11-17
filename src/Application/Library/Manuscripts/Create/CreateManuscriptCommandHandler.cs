using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Manuscripts.Create;

internal sealed class CreateManuscriptCommandHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<CreateManuscriptCommand, int>
{
    public async Task<Result<int>> Handle(CreateManuscriptCommand command, CancellationToken cancellationToken)
    {
        DbSet<Manuscript> manuscripts = unitOfWork.GetDbSet<Manuscript>();


        var manuscript = new Manuscript
        {
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

