using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Cities.Update;

public sealed record UpdateCityCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "نام شهر")] string Name) : ICommand;


