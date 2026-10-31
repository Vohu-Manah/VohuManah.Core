using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.PublicationTypes.Update;

public sealed record UpdatePublicationTypeCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "عنوان نوع نشریه")] string Title) : ICommand;

