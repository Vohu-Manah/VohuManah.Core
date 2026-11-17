using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Attachments.Update;

public sealed record UpdateAttachmentCommand(
    long Id,
    string? Description,
    bool? IsMain) : ICommand;

