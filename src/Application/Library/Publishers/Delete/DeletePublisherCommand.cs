using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Publishers.Delete;

public sealed record DeletePublisherCommand([Display(Name = "شناسه")] int Id) : ICommand;


