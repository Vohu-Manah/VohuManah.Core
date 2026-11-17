using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Attachments.GetByEntity;

public sealed record GetAttachmentsByEntityQuery(
    string EntityType,
    long EntityId) : IQuery<List<AttachmentResponse>>;


