using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Languages.Update;

public sealed record UpdateLanguageCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "نام زبان")] string Name,
    [Display(Name = "مخفف")] string Abbreviation) : ICommand;


