using Application.Abstractions.Data;
using Application.Abstractions.FileStorage;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Attachments.GetByEntity;

internal sealed class GetAttachmentsByEntityQueryHandler(
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService) : IQueryHandler<GetAttachmentsByEntityQuery, List<AttachmentResponse>>
{
    public async Task<Result<List<AttachmentResponse>>> Handle(GetAttachmentsByEntityQuery query, CancellationToken cancellationToken)
    {
        DbSet<Attachment> attachments = unitOfWork.Set<Attachment>();

        List<Attachment> results = await attachments
            .Where(a => a.EntityType == query.EntityType && a.EntityId == query.EntityId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        List<AttachmentResponse> response = results.Select(a => new AttachmentResponse
        {
            Id = a.Id,
            FileName = a.FileName,
            OriginalFileName = a.OriginalFileName,
            FileUrl = fileStorageService.GetFileUrl(a.FilePath),
            ContentType = a.ContentType,
            FileSize = a.FileSize,
            Description = a.Description,
            CreatedAt = a.CreatedAt
        }).ToList();

        return Result.Success(response);
    }
}


