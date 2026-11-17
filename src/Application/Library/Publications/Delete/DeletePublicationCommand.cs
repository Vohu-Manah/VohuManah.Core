using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Publications.Delete;

public sealed record DeletePublicationCommand([Display(Name = "شناسه")] int Id) : ICommand;

