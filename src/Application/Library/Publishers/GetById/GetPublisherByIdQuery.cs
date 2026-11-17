using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Publishers.GetById;

public sealed record GetPublisherByIdQuery(int Id) : IQuery<PublisherResponse>;

