using Application.Abstractions.Messaging;

namespace Application.Library.Publications.GetList;

public sealed record GetPublicationListQuery : IQuery<List<PublicationListResponse>>;

