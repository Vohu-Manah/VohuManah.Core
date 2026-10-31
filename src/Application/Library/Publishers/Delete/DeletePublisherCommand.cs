using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Publishers.Delete;

public sealed record DeletePublisherCommand([Display(Name = "شناسه")] int Id) : ICommand;


