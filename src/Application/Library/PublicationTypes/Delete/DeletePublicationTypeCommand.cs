using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.PublicationTypes.Delete;

public sealed record DeletePublicationTypeCommand([Display(Name = "شناسه")] int Id) : ICommand;

