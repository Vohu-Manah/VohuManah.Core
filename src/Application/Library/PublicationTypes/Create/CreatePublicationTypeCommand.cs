using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.PublicationTypes.Create;

public sealed record CreatePublicationTypeCommand([Display(Name = "عنوان نوع نشریه")] string Title) : ICommand<int>;

