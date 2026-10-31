using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Languages.Create;

public sealed record CreateLanguageCommand(
    [Display(Name = "نام زبان")] string Name,
    [Display(Name = "مخفف")] string Abbreviation) : ICommand<int>;


