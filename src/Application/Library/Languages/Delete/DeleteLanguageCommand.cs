using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Languages.Delete;

public sealed record DeleteLanguageCommand([Display(Name = "شناسه")] int Id) : ICommand;


