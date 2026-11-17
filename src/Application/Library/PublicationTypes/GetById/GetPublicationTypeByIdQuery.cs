using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.PublicationTypes.GetById;

public sealed record GetPublicationTypeByIdQuery(int Id) : IQuery<PublicationTypeResponse>;

