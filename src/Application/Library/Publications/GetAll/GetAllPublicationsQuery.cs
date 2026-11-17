using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Publications.GetAll;

public sealed record GetAllPublicationsQuery() : IQuery<List<PublicationResponse>>;

