using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Subjects.Delete;

public sealed record DeleteSubjectCommand([Display(Name = "شناسه")] int Id) : ICommand;


