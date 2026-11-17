using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Publications;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Publications.Create;

internal sealed class CreatePublicationCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<CreatePublicationCommand, int>
{
    public async Task<Result<int>> Handle(CreatePublicationCommand command, CancellationToken cancellationToken)
    {
        DbSet<Publication> publications = dbContext.Publications;

        if (await publications.AnyAsync(p => p.No == command.No, cancellationToken))
        {
            return Result.Failure<int>(PublicationErrors.NoNotUnique);
        }

        var publication = new Publication
        {
            Name = command.Name,
            TypeId = command.TypeId,
            Concessionaire = command.Concessionaire,
            ResponsibleDirector = command.ResponsibleDirector,
            Editor = command.Editor,
            Year = command.Year,
            JournalNo = command.JournalNo,
            PublishDate = command.PublishDate,
            PublishPlaceId = command.PublishPlaceId,
            No = command.No,
            Period = command.Period,
            LanguageId = command.LanguageId,
            SubjectId = command.SubjectId
        };

        publications.Add(publication);
        await dbContext.SaveChangesAsync(cancellationToken);

        return publication.Id;
    }
}

