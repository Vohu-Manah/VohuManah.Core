using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Menu.Delete;

public sealed record DeleteMenuItemCommand([Display(Name = "شناسه")] int Id) : ICommand;

