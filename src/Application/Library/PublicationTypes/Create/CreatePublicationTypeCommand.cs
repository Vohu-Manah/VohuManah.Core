using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.PublicationTypes.Create;

public sealed record CreatePublicationTypeCommand([Display(Name = "عنوان نوع نشریه")] string Title) : ICommand<int>;

