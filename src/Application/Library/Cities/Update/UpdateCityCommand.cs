using Application.Abstractions.Messaging;

namespace Application.Library.Cities.Update;

public sealed record UpdateCityCommand(int Id, string Name) : ICommand;


