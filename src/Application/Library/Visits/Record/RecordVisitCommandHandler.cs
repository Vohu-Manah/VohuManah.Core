using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Visits.Record;

internal sealed class RecordVisitCommandHandler(
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<RecordVisitCommand>
{
    public async Task<Result> Handle(RecordVisitCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Path) || command.Path.Length > 300)
        {
            return Result.Success();
        }

        var visitLog = new VisitLog
        {
            Path = command.Path,
            VisitedAt = dateTimeProvider.UtcNow
        };

        await unitOfWork.GetDbSet<VisitLog>().AddAsync(visitLog, cancellationToken).ConfigureAwait(false);
        await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
