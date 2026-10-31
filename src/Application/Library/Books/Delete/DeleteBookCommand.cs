using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Books.Delete;

public sealed record DeleteBookCommand([Display(Name = "شناسه")] long Id) : ICommand;

