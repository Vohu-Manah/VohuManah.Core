using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Subjects.Create;

public sealed record CreateSubjectCommand([Display(Name = "عنوان موضوع")] string Title) : ICommand<int>;


