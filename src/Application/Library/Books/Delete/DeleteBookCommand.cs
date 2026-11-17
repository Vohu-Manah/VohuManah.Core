using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Books.Delete;

public sealed record DeleteBookCommand([Display(Name = "شناسه")] long Id) : ICommand;

