using Application.Abstractions.Messaging;

namespace Application.Library.Publications.GetById;

public sealed record GetPublicationByIdQuery(int Id) : IQuery<PublicationResponse>;

