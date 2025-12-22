using Application.Abstractions.Messaging;

namespace Application.Library.Attachments.Delete;

public sealed record DeleteAttachmentCommand(long Id) : ICommand;


