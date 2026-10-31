using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Publications.Delete;

public sealed record DeletePublicationCommand([Display(Name = "شناسه")] int Id) : ICommand;

