using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Cities.Delete;

public sealed record DeleteCityCommand([Display(Name = "شناسه")] int Id) : ICommand;


