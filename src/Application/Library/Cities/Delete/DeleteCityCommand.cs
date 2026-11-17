using VohuManah.Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace VohuManah.Application.Library.Cities.Delete;

public sealed record DeleteCityCommand([Display(Name = "شناسه")] int Id) : ICommand;


