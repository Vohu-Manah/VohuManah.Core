using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.Update;

public sealed record UpdatePublisherCommand(
    int Id,
    string Name,
    string ManagerName,
    int PlaceId,
    string Address,
    string Telephone,
    string Website,
    string Email) : ICommand;


