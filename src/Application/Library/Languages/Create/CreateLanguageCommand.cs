using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Languages.Create;

public sealed record CreateLanguageCommand(
    [Display(Name = "نام زبان")] string Name,
    [Display(Name = "مخفف")] string Abbreviation) : ICommand<int>;


