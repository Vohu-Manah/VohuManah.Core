using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Publications.GetById;

public sealed record GetPublicationByIdQuery(int Id) : IQuery<PublicationResponse>;

