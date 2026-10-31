using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Cities.Update;

public sealed record UpdateCityCommand(
    [Display(Name = "شناسه")] int Id,
    [Display(Name = "نام شهر")] string Name) : ICommand;


