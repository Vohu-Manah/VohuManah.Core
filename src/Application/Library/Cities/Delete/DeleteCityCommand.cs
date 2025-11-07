using Application.Abstractions.Messaging;

namespace Application.Library.Cities.Delete;

public sealed record DeleteCityCommand(int Id) : ICommand;


