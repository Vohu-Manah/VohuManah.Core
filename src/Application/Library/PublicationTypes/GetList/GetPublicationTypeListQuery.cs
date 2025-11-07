using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.GetList;

public sealed record GetPublicationTypeListQuery : IQuery<List<PublicationTypeListResponse>>;

