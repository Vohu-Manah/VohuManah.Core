using Application.Abstractions.Messaging;

namespace Application.Library.Cities.Create;

public sealed record CreateCityCommand(string Name) : ICommand<int>;


