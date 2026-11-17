using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Menu.Delete;

public sealed record DeleteMenuItemCommand([Display(Name = "شناسه")] int Id) : ICommand;

