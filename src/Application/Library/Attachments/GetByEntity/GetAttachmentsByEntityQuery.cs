using Application.Abstractions.Messaging;

namespace Application.Library.Attachments.GetByEntity;

public sealed record GetAttachmentsByEntityQuery(
    string EntityType,
    long EntityId) : IQuery<List<AttachmentResponse>>;


