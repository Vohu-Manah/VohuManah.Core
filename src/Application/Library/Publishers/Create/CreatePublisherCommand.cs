using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.Create;

public sealed record CreatePublisherCommand(
    string Name,
    string ManagerName,
    int PlaceId,
    string Address,
    string Telephone,
    string Website,
    string Email) : ICommand<int>;


