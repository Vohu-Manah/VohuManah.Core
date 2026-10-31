using Application.Abstractions.Messaging;

namespace Application.Library.Attachments.Update;

public sealed record UpdateAttachmentCommand(
    long Id,
    string? Description,
    bool? IsMain) : ICommand;

