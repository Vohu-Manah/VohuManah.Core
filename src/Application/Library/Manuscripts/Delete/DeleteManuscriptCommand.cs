using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Manuscripts.Delete;

public sealed record DeleteManuscriptCommand([Display(Name = "شناسه")] int Id) : ICommand;

