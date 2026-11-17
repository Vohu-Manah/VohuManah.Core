using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Attachments.Delete;

public sealed record DeleteAttachmentCommand([Display(Name = "شناسه")] long Id) : ICommand;


