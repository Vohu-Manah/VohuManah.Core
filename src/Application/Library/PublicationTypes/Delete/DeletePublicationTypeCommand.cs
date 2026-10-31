using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.PublicationTypes.Delete;

public sealed record DeletePublicationTypeCommand([Display(Name = "شناسه")] int Id) : ICommand;

