using Application.Abstractions.Messaging;

namespace Application.Library.Attachments.Upload;

public sealed record UploadAttachmentCommand(
    string EntityType,
    long EntityId,
    string FileName,
    string ContentType,
    long FileSize,
    Stream FileStream,
    string? Description,
    string CreatedBy) : ICommand<long>;


