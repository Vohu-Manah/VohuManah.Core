using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.PublicationTypes.GetList;

public sealed record GetPublicationTypeListQuery : IQuery<List<PublicationTypeListResponse>>;

