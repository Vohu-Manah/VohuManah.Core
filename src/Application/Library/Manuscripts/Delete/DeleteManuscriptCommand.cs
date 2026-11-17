using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Manuscripts.Delete;

public sealed record DeleteManuscriptCommand([Display(Name = "شناسه")] int Id) : ICommand;

