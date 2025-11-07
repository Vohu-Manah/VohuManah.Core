using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.GetAll;

public sealed record GetAllPublicationTypesQuery() : IQuery<List<PublicationTypeResponse>>;

