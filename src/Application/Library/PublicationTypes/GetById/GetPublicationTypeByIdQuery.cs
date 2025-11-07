using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.GetById;

public sealed record GetPublicationTypeByIdQuery(int Id) : IQuery<PublicationTypeResponse>;

