using Application.Abstractions.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Application.Library.Cities.Create;

public sealed record CreateCityCommand([Display(Name = "نام شهر")] string Name) : ICommand<int>;


