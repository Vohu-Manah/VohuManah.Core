using Application.Abstractions.Messaging;

namespace Application.Library.Publications.GetAll;

public sealed record GetAllPublicationsQuery() : IQuery<List<PublicationResponse>>;

