using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Subjects.Delete;

public sealed record DeleteSubjectCommand([Display(Name = "شناسه")] int Id) : ICommand;


